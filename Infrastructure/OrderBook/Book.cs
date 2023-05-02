using Application.Common.Models;
using Domain.Entities;
using Domain.ValueObjects;

public class Book : IBook
{
    private readonly Side<QuantityTrackingPriceLevel> _bids;
    private readonly Side<QuantityTrackingPriceLevel> _asks;
    private long _sequence;
    public IEnumerable<QuantityTrackingPriceLevel> BidSide => _bids.PriceLevels;
    public IEnumerable<QuantityTrackingPriceLevel> AskSide => _asks.PriceLevels;
    internal int AskPriceLevelCount => _asks.PriceLevelCount;
    internal int BidPriceLevelCount => _bids.PriceLevelCount;


    public Book()
    {
        _bids = new(PriceComparerDescending.Instance, PriceLevelComparerDescending.Instance);
        _asks = new(PriceComparerAscending.Instance, PriceLevelComparerAscending.Instance);
    }

    public bool RemoveOrder(IOrder order, Price price)
    {
        if (order is OrderMarket marketOrder)
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
    private bool RemoveMarketOrder(OrderMarket order)
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
        if (order is OrderMarket marketOrder)
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
    private void AddMarketOrder(OrderMarket order)
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