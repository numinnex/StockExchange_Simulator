using Domain.Entities;
using Domain.ValueObjects;

public class Book : IBook
{
    private readonly Side<QuantityTrackingPriceLevel> _bids;
    private readonly Side<QuantityTrackingPriceLevel> _asks;
    private long _sequence;
    public IEnumerable<QuantityTrackingPriceLevel> BidSide => _bids.PriceLevels;
    public IEnumerable<QuantityTrackingPriceLevel> AskSide => _asks.PriceLevels;
    public int BidLevelsCount => _bids.PriceLevelCount;
    public int AskLevelsCount => _asks.PriceLevelCount; 
    public int AskPriceLevelCount => _asks.PriceLevelCount;
    public int BidPriceLevelCount => _bids.PriceLevelCount;

    public Book()
    {
        _bids = new(PriceComparerDescending.Instance, PriceLevelComparerDescending.Instance);
        _asks = new(PriceComparerAscending.Instance, PriceLevelComparerAscending.Instance);
    }

    public bool RemoveOrder(IOrder order, Price price)
    {
        if (order is MarketOrder marketOrder)
        {
            var removed = RemoveMarketOrder(marketOrder);
            if (!removed)
            {
                return false;
            }
            return true;
        }
        return true;
    }
    private bool RemoveMarketOrder(MarketOrder order)
    {
        var removed = false;
        if (order.IsBuy)
        {
            removed = _bids.RemoveOrder(order, order.Price);
            return removed;
        }
        removed = _asks.RemoveOrder(order, order.Price);
        return removed;
    }

    public bool AddOrder(IOrder order, Price price)
    {
        if (order is MarketOrder marketOrder)
        {
            try
            {
                AddMarketOrder(marketOrder);
            }
            catch
            {
                return false;
            }
            return true;
        }
        return true;
    }
    public IOrder? GetBestOffer(bool isBuy, string userId)
    {
        return isBuy
            ? _bids.BestPriceLevel?.FirstOrDefault(x => x.UserId != userId)
            : _asks.BestPriceLevel?.FirstOrDefault(x => x.UserId != userId);
    }
    private void AddMarketOrder(MarketOrder order)
    {
        var side = order.IsBuy ? _bids : _asks;
        side.AddOrder(order, order.Price);
    }

    public bool FillOrder(IOrder order, Quantity quantity, Price price)
    {
        var side = order.IsBuy ? _bids : _asks;
        return side.FillOrder(order, quantity, price);
    }

    public bool CheckCanFillMarketOrderAmount(bool isBuy, Amount orderAmount)
    {
        var side = isBuy ? _bids : _asks;
        return side.CheckMarketOrderAmountCanBeFilled(orderAmount);
    }
}