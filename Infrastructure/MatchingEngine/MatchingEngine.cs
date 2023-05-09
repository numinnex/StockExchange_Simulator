using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

public class MatchingEngine : IMatchingEngine
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Dictionary<string, IBook> _books;

    public MatchingEngine( 
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _books = new();
    }
    public async Task AddOrder(IOrder order)
    {
        if (order is MarketOrder marketOrder)
        {
            if (marketOrder.OpenQuantity is not null)
            {
                await HandleMarketOrderWithOpenQuantity(marketOrder);
            }
            if (marketOrder.OrderAmount is not null)
            {

            }
        }
    }
    private async Task HandleMarketOrderWithOpenQuantity(MarketOrder marketOrder)
    {
        if (!_books.ContainsKey(marketOrder.Symbol))
        {
            _books.Add(marketOrder.Symbol, new Book());
        }
        var book = _books[marketOrder.Symbol];
        var matched = await MatchWithOpenOrders(marketOrder, book);
        var result = book.AddOrder(marketOrder, marketOrder.Price);
        if (result)
        {
            System.Console.WriteLine("Order added successfully");
        }
    }
    private async Task<bool> MatchWithOpenOrders(IOrder incommingOrder, IBook book)
    {
        while (true)
        {
            if (incommingOrder is MarketOrder incommingMarketOrder)
            {
                var restingMarketOrder = book.GetBestOffer(!incommingMarketOrder.IsBuy) as MarketOrder;
                if (restingMarketOrder is null)
                {
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

                        var cost = Math.Round(maxQuantity * matchPrice, 4);
                        incommingMarketOrder.Cost += cost;
                        restingMarketOrder.Cost += cost;
                        
                        using var scope = _serviceScopeFactory.CreateScope();
                        var feeProvider = scope.ServiceProvider.GetService<IFeeProvider>();
                        
                        //TODO - handle the null error that GetFee throws given non-existing FeeId
                        //I Will validate the feeIds in the order handler methods
                        var incommingFee = await feeProvider!.GetFee(incommingOrder.FeeId);
                        var restingFee = await feeProvider.GetFee(restingMarketOrder.FeeId);
                        restingMarketOrder.FeeAmount += Math.Round((cost * restingFee.MakerFee) / 100, 4);
                        incommingMarketOrder.FeeAmount += Math.Round((cost * incommingFee.TakerFee) / 100, 4);

                        //This method subtracts quantity from the resting order 
                        var isRestingOrderFilled = book.FillOrder(restingMarketOrder,
                            maxQuantity,
                            matchPrice);
                        var isIncommingOrderFilled = incommingMarketOrder.IsFilled;

                        //TODO - Create a trade footprint class
                        CreateTradeFootprint(
                            restingMarketOrder.FeeAmount,
                            incommingMarketOrder.FeeAmount,
                            cost,
                            incommingMarketOrder,
                            restingMarketOrder,
                            isRestingOrderFilled,
                            isIncommingOrderFilled);
                    }
                }
                else
                {
                    return false;
                }
                if (incommingMarketOrder.IsFilled)
                {
                    return true;
                }
            }
        }
    }

    private void CreateTradeFootprint(
        decimal restingOrderFee,
        decimal incommingOrderFee,
        decimal cost,
        MarketOrder incommingMarketOrder,
        MarketOrder restingMarketOrder,
        bool isRestingOrderFilled,
        bool isIncommingOrderFilled)
    {
        //TODO - refactor this into more organized structure (no unnecessary null field props)
        Quantity? askRemainingQuantity;
        Amount? askFee;
        Amount? bidFee;
        Amount? bidCost;

        if (incommingMarketOrder.IsBuy)
        {
            if (isIncommingOrderFilled)
            {
                bidCost = cost;
                bidFee = incommingOrderFee;
            }
            if (isRestingOrderFilled)
            {
                askRemainingQuantity = restingMarketOrder.OpenQuantity;
                askFee = restingOrderFee;
            }
        }
        else
        {
            if (isRestingOrderFilled)
            {
                bidCost = cost;
                bidFee = restingOrderFee;
            }
            if (isIncommingOrderFilled)
            {
                askRemainingQuantity = incommingMarketOrder.OpenQuantity;
                askFee = incommingOrderFee;
            }
        }
    }

    public void CancelOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }
}