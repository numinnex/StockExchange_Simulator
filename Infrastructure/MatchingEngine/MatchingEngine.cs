using System.Collections.Concurrent;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.PriceLevels;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MatchingEngine;

public class MatchingEngine : IMatchingEngine
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    //This can be solved differently 
    //1) Create a self hosted Book project in docker, that will expose api through http to 
    //add orders/remove orders and maintain the state of the book
    //2) Create a IBookProvider and IQueueProvider that will expose api to manage book and queue
    private readonly ConcurrentDictionary<string, IBook> _books;
    private readonly ConcurrentDictionary<string, Queue<IReadOnlyList<PriceLevel>>> _stopOrdersQueue;
    private readonly Quantity _stepSize;
    
    [ActivatorUtilitiesConstructor]
    public MatchingEngine( 
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        //TODO - initialize the stopOrderQueues and books from database
        _stopOrdersQueue = new();
        _books = new();
        //TODO - create options pattern for matching engine settings (move to appsettings.json) 
        _stepSize = new (0.0000_0001m);
    }

    public MatchingEngine(IServiceScopeFactory serviceScopeFactory, IBook doubleBook,
        Queue<IReadOnlyList<PriceLevel>> doubleStopOrderQueue)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _stopOrdersQueue = new();
        _books = new();
        _books["APPL"] = doubleBook;
        _stopOrdersQueue["APPL"] = doubleStopOrderQueue;
        _stepSize = new (0.0000_0001m);
    }
    //TODO - Create a constructor from which will inject book and queue as dependencies for double testing
    //   _books["default"] = book;
    //   _stopOrdersQueue["default"] = queue;
    //TODO - Change return type to OrderProcessingResult
    public async Task<Ok<OrderCreationResult?, StopOrderProcessingResult?>> AddOrder(IOrder order, CancellationToken token)
    {
        var book = GetOrCreateBook(order.Symbol);
        var queue = GetOrCreateQueue(order.Symbol);
        if (order is MarketOrder marketOrder)
        {
            if (marketOrder.OpenQuantity is not null)
            {
                var result =  await HandleMarketOrderWithOpenQuantity(marketOrder , token, book,queue);
                var triggeredOrdersResult = await MatchAndAddTriggeredOrders(queue, book, token);
                return new Ok<OrderCreationResult?, StopOrderProcessingResult?>(result, triggeredOrdersResult);
            }
            if (marketOrder.OrderAmount is not null)
            {
                var result = await HandleMarketOrderWithAmount(marketOrder, token, book, queue);
                var triggeredOrdersResult = await MatchAndAddTriggeredOrders(queue, book, token);
                return new Ok<OrderCreationResult?, StopOrderProcessingResult?>(result, triggeredOrdersResult);
            }
        }

        if (order is StopOrder stopOrder)
        {
            if (stopOrder.OpenQuantity is not null)
            {
                await HandleStopOrder(stopOrder, token, book);
                return new Ok<OrderCreationResult?, StopOrderProcessingResult?>(
                null, null, true
            );
            }

        }
        //get rid of this later
        return new Ok<OrderCreationResult?, StopOrderProcessingResult?>(
            new OrderCreationResult
            {
                IsFilled = false,
                IsMatched = false,

            },
            new StopOrderProcessingResult
            {
                FilledCount = 0,
                MatchedCount = 0,
                CostAccumulate = 0
            },
            false
        );
    }
    //TODO - change this return type to something less bloated, I will use something else for testing
    private async Task<OrderCreationResult> HandleMarketOrderWithAmount(MarketOrder order, CancellationToken token,
        IBook book, Queue<IReadOnlyList<PriceLevel>> queue)
    {
        var quantityByOrderAmount = GetQuantityByOrderAmount(order.OrderAmount!, book);
        var previousMarketPrice = book.CurrentMarketPrice;
        
        if (quantityByOrderAmount is not null)
        {
            order.OpenQuantity = quantityByOrderAmount;
            var matchingResult = await MatchWithOpenOrders(order, book, token);
            if (!matchingResult.IsFilled)
            {
                AddOrderToBook(book, order);
            }
            
            AddStopOrdersToQueue(previousMarketPrice, book, queue);
            
            return new OrderCreationResult
            {
                IsFilled = matchingResult.IsFilled,
                IsMatched = matchingResult.IsMatched,
                Cost = matchingResult.Cost,
                AskLevelsCount = book.AskLevelsCount,
                BidLevelsCount = book.BidLevelsCount,
                BidsCount = book.BestBidsCount,
                AsksCount = book.BestAsksCount
            };
        }

        return new OrderCreationResult
        {
            IsFilled = false,
            IsMatched = false,
        };
    }

    private static void AddStopOrdersToQueue(Price? previousMarketPrice, IBook book,
        Queue<IReadOnlyList<PriceLevel>> queue)
    {
        if (previousMarketPrice is not null)
        {
            var currentMarketPrice = book.CurrentMarketPrice!;
            if (currentMarketPrice > previousMarketPrice)
            {
                var stopBidsPriceLevel = book.GetStopBids(currentMarketPrice);
                queue.Enqueue(stopBidsPriceLevel);
            }
            else if (currentMarketPrice < previousMarketPrice)
            {
                var stopAsksPriceLevel = book.GetStopAsks(currentMarketPrice);
                queue.Enqueue(stopAsksPriceLevel);
            }
        }
    }

    private async Task<bool> HandleStopOrder(StopOrder order, CancellationToken token, IBook book)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetService<IOrderRepository>();
        var result = AddOrderToBook(book, order);
        if (result)
        {
            await orderRepository!.AddAsync(order, token);
        }

        return result;
    }
    //TODO - change return type there aswell
    private async Task<OrderCreationResult> HandleMarketOrderWithOpenQuantity(MarketOrder order,
        CancellationToken token, IBook book , Queue<IReadOnlyList<PriceLevel>> queue)
    {
        var previousMarketPrice = book.CurrentMarketPrice;
        //MatchWithOpenOrders changes currentMarketPrice
        var matchingResult = await MatchWithOpenOrders(order, book, token);
        if (!matchingResult.IsFilled)
        {
            AddOrderToBook(book, order);
        }

        AddStopOrdersToQueue(previousMarketPrice, book , queue);
        
        return new OrderCreationResult
        {
            IsFilled = matchingResult.IsFilled,
            IsMatched = matchingResult.IsMatched,
            Cost = matchingResult.Cost,
            AskLevelsCount = book.AskLevelsCount,
            BidLevelsCount = book.BidLevelsCount,
            BidsCount = book.BestBidsCount,
            AsksCount = book.BestAsksCount
        };
    }
    

    private async Task<StopOrderProcessingResult> MatchAndAddTriggeredOrders(Queue<IReadOnlyList<PriceLevel>> queue, IBook book, CancellationToken token)
    {
        decimal filledCount = 0m;  
        int matchedCount = 0;
        decimal costAccum = 0;
        while (queue.TryDequeue(out var priceLevels))
        {
            for (var i = 0; i < priceLevels.Count; i++)
            {
                using var iterator = priceLevels[i].GetEnumerator();
                while (iterator.MoveNext())
                {
                    var order = iterator.Current;
                    if (order is StopOrder stopOrder)
                    {
                        var result = await HandleTriggeredStopOrders(stopOrder, token, book, queue);
                        if (result.IsFilled) filledCount++;
                        if (result.IsMatched) matchedCount++;
                        costAccum += result.Cost;
                    }
                }
            }
        }
        return new StopOrderProcessingResult
        {
            CostAccumulate = costAccum,
            FilledCount = filledCount,
            MatchedCount = matchedCount
        };
    }
    private async Task<StopOrderTriggeredResult> HandleTriggeredStopOrders(StopOrder order, CancellationToken token,
        IBook book,
        Queue<IReadOnlyList<PriceLevel>> queue)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetService<IOrderRepository>();
        
        order.IsTriggered = true;
        var marketOrder = new MarketOrder
        {
            Id = Guid.NewGuid(),
            Symbol = order.Symbol,
            Price = order.StopPrice,
            Timestamp = order.Timestamp,
            OpenQuantity = order.OpenQuantity,
            OrderAmount = null,
            FeeId = order.FeeId,
            IsBuy = order.IsBuy,
            StockId = order.StockId,
            UserId = order.UserId,
            Type = order.Type == TradeType.StopBuy ? TradeType.BuyMarket : TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
        };
        var result = await HandleMarketOrderWithOpenQuantity(marketOrder, token, book ,queue);
        if (result.IsFilled || result.IsMatched)
            orderRepository!.Remove(order);
        
        return new StopOrderTriggeredResult
        {
            Cost = result.Cost ?? 0,
            IsFilled = result.IsFilled,
            IsMatched = result.IsMatched
        };
    }
    private void CreateBookWithRetries(string symbol)
    {
        const int maxRetires = 5;
        int retryCount = 0;
        while (retryCount < maxRetires)
        {
            if (_books.TryAdd(symbol, new Book()))
            {
                break;        
            }
            retryCount++;
            Thread.Sleep(500);
        }
    }
    private void CreateQueueWithRetries(string symbol)
    {
        const int maxRetires = 5;
        int retryCount = 0;
        while (retryCount < maxRetires)
        {
            if (_stopOrdersQueue.TryAdd(symbol, new Queue<IReadOnlyList<PriceLevel>>()))
            {
                break;        
            }
            retryCount++;
            Thread.Sleep(500);
        }
    }

    private Queue<IReadOnlyList<PriceLevel>> GetOrCreateQueue(string symbol)
    {
        if (!_stopOrdersQueue.ContainsKey(symbol))
        {
            CreateQueueWithRetries(symbol);
        }
        return _stopOrdersQueue[symbol];
    }
    private IBook GetOrCreateBook(string symbol)
    {
        if (!_books.ContainsKey(symbol))
        {
            CreateBookWithRetries(symbol);
        }

        return _books[symbol];
    }
    private async Task<MatchOrderResult> MatchWithOpenOrders(IOrder incommingOrder, IBook book, CancellationToken token)
    {
        int iterCount = 0;
        while (true)
        {
            //TODO - get rid of this if statement later
            if (incommingOrder is MarketOrder incommingMarketOrder)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var orderRepository = scope.ServiceProvider.GetService<IOrderRepository>();
                var tradeListener = scope.ServiceProvider.GetService<ITradeListener>();

                var restingMarketOrder =
                    book.GetBestOffer(!incommingMarketOrder.IsBuy, incommingMarketOrder.UserId) as MarketOrder;
                if (restingMarketOrder is null)
                {
                    await tradeListener!.OnAcceptAsync(incommingMarketOrder, token);
                    return new MatchOrderResult
                    {
                        IsFilled = false,
                        IsMatched = iterCount > 0,
                        Cost = incommingMarketOrder.Cost
                    };
                }

                if ((incommingMarketOrder.IsBuy && restingMarketOrder.Price <= incommingMarketOrder.Price)
                    || (!incommingMarketOrder.IsBuy && restingMarketOrder.Price >= incommingMarketOrder.Price))
                {
                    iterCount++;
                    var matchPrice = restingMarketOrder.Price;
                    Quantity maxQuantity;

                    if (incommingMarketOrder.OpenQuantity! > 0)
                    {
                        maxQuantity = Math.Min(incommingMarketOrder.OpenQuantity!,
                            restingMarketOrder.OpenQuantity!);

                        incommingMarketOrder.OpenQuantity -= maxQuantity;
                        incommingMarketOrder.UpdateTradeStatus();

                        var cost = Math.Round(maxQuantity * matchPrice, 4);
                        incommingMarketOrder.Cost += cost;
                        restingMarketOrder.Cost += cost;

                        var feeProvider = scope.ServiceProvider.GetService<IFeeProvider>();

                        var incommingFee = await feeProvider!.GetFeeAsync(incommingOrder.FeeId);
                        var restingFee = await feeProvider.GetFeeAsync(restingMarketOrder.FeeId);
                        if (restingFee is not null)
                            restingMarketOrder.FeeAmount += Math.Round((cost * restingFee.MakerFee) / 100, 4);
                        if (incommingFee is not null)
                            incommingMarketOrder.FeeAmount += Math.Round((cost * incommingFee.TakerFee) / 100, 4);

                        //This method subtracts quantity from the resting order 
                        var isRestingOrderFilled = book.FillOrder(restingMarketOrder,
                            maxQuantity,
                            matchPrice);
                        restingMarketOrder.UpdateTradeStatus();
                        var isIncommingOrderFilled = incommingMarketOrder.IsFilled;

                        orderRepository!.Update(restingMarketOrder);

                        var tradeFootprint = CreateTradeFootprint(matchPrice, maxQuantity, incommingMarketOrder,
                            restingMarketOrder, isRestingOrderFilled, isIncommingOrderFilled);
                        await tradeListener!.OnTradeAsync(tradeFootprint, token);

                        if (isIncommingOrderFilled)
                        {
                            await tradeListener!.OnAcceptAsync(incommingMarketOrder, token);
                            return new MatchOrderResult
                            {
                                IsFilled = true,
                                IsMatched = true,
                                Cost = incommingMarketOrder.Cost
                            };
                        }
                    }
                }
                else
                {
                    await tradeListener!.OnAcceptAsync(incommingMarketOrder, token);
                    return new MatchOrderResult
                    {
                        IsFilled = false,
                        IsMatched = iterCount > 0,
                        Cost = incommingMarketOrder.Cost
                    };
                }
            }
        }
    }

    private bool AddOrderToBook(IBook book, IOrder incommingMarketOrder)
    {
        return book.AddOrder(incommingMarketOrder);
    }

    private TradeFootprint CreateTradeFootprint(
        Price matchPrice,
        Quantity maxQuantity,
        MarketOrder incommingMarketOrder,
        MarketOrder restingMarketOrder,
        bool isRestingOrderFilled,
        bool isIncommingOrderFilled)
    {
        Quantity? askRemainingQuantity = null;
        Amount? askFee = null;
        Amount? bidFee = null;
        Amount? bidCost = null;

        if (incommingMarketOrder.IsBuy)
        {
            if (isIncommingOrderFilled)
            {
                bidCost = incommingMarketOrder.Cost;
                bidFee = incommingMarketOrder.FeeAmount;
            }
            
            switch (isRestingOrderFilled)
            {
                case true:
                    askFee = restingMarketOrder.FeeAmount;
                    break;
                case false:
                    askRemainingQuantity = restingMarketOrder.OpenQuantity;
                    break;
            }
        }
        else
        {
            if (isRestingOrderFilled)
            {
                bidCost = restingMarketOrder.Cost;
                bidFee = restingMarketOrder.FeeAmount;
            }
            switch (isIncommingOrderFilled)
            {
                case true:
                    askFee = incommingMarketOrder.FeeAmount;
                    break;
                case false:
                    askRemainingQuantity = incommingMarketOrder.OpenQuantity;
                    break;
            }
        }
        return new TradeFootprint
        {
            Quantity = maxQuantity,
            MatchPrice = matchPrice,
            ProcessedOrderId = incommingMarketOrder.Id,
            RestingOrderId = restingMarketOrder.Id,
            ProcessedOrderIsBuy = incommingMarketOrder.IsBuy,
            ProcessedOrderUserId = incommingMarketOrder.UserId,
            RestingOrderUserId = restingMarketOrder.UserId,
            TradeDetails = new TradeDetails
            {
                AskFee = askFee,
                BidCost = bidCost,
                BidFee = bidFee,
                RemainingQuantity = askRemainingQuantity
            }
        };
    }

    private Quantity? GetQuantityByOrderAmount(Amount orderAmount, IBook book)
    {
        var dustRemaining = false;
        Quantity quantity = 0;
        decimal q;
        using var enumerator = book.AskSide.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var level = enumerator.Current;
            using var levelEnumerator = level.GetEnumerator();
            while (levelEnumerator.MoveNext())
            {
                var order = levelEnumerator.Current;
                if (order is not MarketOrder marketOrder) 
                    continue;
                if (orderAmount == 0)
                    goto breakOutside;
                if (marketOrder.OpenQuantity is null)
                    continue;

                var amount = marketOrder.OpenQuantity * marketOrder.Price;
                if (amount <= orderAmount)
                {
                    quantity += marketOrder.OpenQuantity;
                    orderAmount -= amount;
                }
                else
                {
                    q = (orderAmount / marketOrder.Price);
                    q -= (q % _stepSize);
                    if (q > 0)
                    {
                        quantity += q;
                        orderAmount -= (q * marketOrder.Price);
                    }
                    else
                    {
                        goto breakOutside;
                    }
                }

            }
        }
        breakOutside:
        if (quantity > 0)
            return quantity;
        return null;
    }

    public void CancelOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }
}