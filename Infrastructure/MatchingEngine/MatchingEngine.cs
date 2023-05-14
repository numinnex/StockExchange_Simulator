using System.Collections.Concurrent;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

public class MatchingEngine : IMatchingEngine
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ConcurrentDictionary<string, IBook> _books;

    private readonly Quantity _stepSize;
    
    public MatchingEngine( 
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _books = new();
        //TODO - create options pattern for matchingengine settings (move to appsettings.json) 
        _stepSize = new (0.0000_0001m);
    }
    public async Task AddOrder(IOrder order, CancellationToken token)
    {
        if (order is MarketOrder marketOrder)
        {
            if (marketOrder.OpenQuantity is not null)
            {
                await HandleMarketOrderWithOpenQuantity(marketOrder , token);
            }
            if (marketOrder.OrderAmount is not null)
            {
                await HandleMarketOrderWithAmount(marketOrder, token);
            }
        }
    }
    private async Task HandleMarketOrderWithAmount(MarketOrder order, CancellationToken token)
    {
        var book = GetOrCreateBook(order.Symbol);
        var quantityByOrderAmount = GetQuantityByOrderAmount(order.OrderAmount!, book);
        if (quantityByOrderAmount is not null)
        {
            order.OpenQuantity = quantityByOrderAmount;
            var matched = await MatchWithOpenOrders(order, book, token);
            if (!matched)
            {
                var result = book.AddOrder(order, order.Price);
            }
        }
    }
    private async Task HandleMarketOrderWithOpenQuantity(MarketOrder order, CancellationToken token)
    {
        var book = GetOrCreateBook(order.Symbol);
        var matched = await MatchWithOpenOrders(order, book, token);
        if (!matched )
        {
            var result = book.AddOrder(order, order.Price);
            if (result)
            {
                
            }
        }
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
            else
            {
                retryCount++;
                Thread.Sleep(500);
            }
        }
    }
    private IBook GetOrCreateBook(string symbol)
    {
        if (!_books.ContainsKey(symbol))
        {
            CreateBookWithRetries(symbol);
        }

        return _books[symbol];
    }
    //TODO - change the return type from bool to something more elaborative 
    private async Task<bool> MatchWithOpenOrders(IOrder incommingOrder, IBook book, CancellationToken token)
    {
        while (true)
        {
            if (incommingOrder is MarketOrder incommingMarketOrder)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var orderRepository = scope.ServiceProvider.GetService<IOrderRepository>();
                var tradeListener = scope.ServiceProvider.GetService<ITradeListener>();
                
                var restingMarketOrder = book.GetBestOffer(!incommingMarketOrder.IsBuy) as MarketOrder;
                if (restingMarketOrder is null)
                {
                    await tradeListener!.OnAcceptAsync(incommingMarketOrder, token);
                    return false;
                }
                if ((incommingMarketOrder.IsBuy && restingMarketOrder.Price <= incommingMarketOrder.Price)
                    || (!incommingMarketOrder.IsBuy && restingMarketOrder.Price >= incommingMarketOrder.Price))
                {
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
                        
                        var incommingFee = await feeProvider!.GetFee(incommingOrder.FeeId);
                        var restingFee = await feeProvider.GetFee(restingMarketOrder.FeeId);
                        if(restingFee is not null)
                            restingMarketOrder.FeeAmount += Math.Round((cost * restingFee.MakerFee) / 100, 4);
                        if(incommingFee is not null)
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
                        
                    }
                }
                else
                {
                    await tradeListener!.OnAcceptAsync(incommingMarketOrder, token);
                    return false;
                }
                if (incommingMarketOrder.IsFilled)
                {
                    await tradeListener!.OnAcceptAsync(incommingMarketOrder, token);
                    return true;
                }
            }
        }
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
                    dustRemaining = true;
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