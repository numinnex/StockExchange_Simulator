using Domain.Entities;
using Domain.PriceLevels;
using Domain.ValueObjects;

public class Book : IBook
{
    private readonly object _sync = new object();
    
    private readonly Side<QuantityTrackingPriceLevel> _bids;
    private readonly Side<QuantityTrackingPriceLevel> _asks;
    private readonly Side<PriceLevel> _stopBids;
    private readonly Side<PriceLevel> _stopAsks;
    private Price? _currentMarketPrice;
    private long _sequence;
    public IEnumerable<QuantityTrackingPriceLevel> BidSide => _bids.PriceLevels;
    public IEnumerable<QuantityTrackingPriceLevel> AskSide => _asks.PriceLevels;
    public IEnumerable<PriceLevel> StopBidSide => _stopBids.PriceLevels;
    public IEnumerable<PriceLevel> StopAskSide => _stopAsks.PriceLevels;
    public Price? CurrentMarketPrice => _currentMarketPrice;
    public Price? BestStopBidPrice => _stopBids.BestPriceLevel?.Price;
    public Price? BestStopAskPrice => _stopAsks.BestPriceLevel?.Price;
    public int BidLevelsCount => _bids.PriceLevelCount;
    public int AskLevelsCount => _asks.PriceLevelCount;
    public int BestBidsCount => _bids.BestPriceLevel?.OrderCount ?? 0;
    public int BestAsksCount => _asks.BestPriceLevel?.OrderCount ?? 0;
    public int StopBidsLevelCount => _stopBids.PriceLevelCount;
    public int StopAsksLevelCount => _stopAsks.PriceLevelCount; 
    public int BestStopBidsCount => _stopBids.BestPriceLevel?.OrderCount ?? 0;
    public int BestStopAsksCount => _stopAsks.BestPriceLevel?.OrderCount ?? 0;

    public Book()
    {
        _bids = new(PriceComparerDescending.Instance, PriceLevelComparerDescending.Instance);
        _asks = new(PriceComparerAscending.Instance, PriceLevelComparerAscending.Instance);
        _stopBids = new(PriceComparerAscending.Instance, PriceLevelComparerAscending.Instance);
        _stopAsks = new(PriceComparerDescending.Instance, PriceLevelComparerDescending.Instance);
    }

    public bool RemoveOrder(IOrder order, Price price)
    {
        if (order is MarketOrder marketOrder)
        {
            lock (_sync)
            {
                var removed = RemoveMarketOrder(marketOrder);
                if (!removed)
                {
                    return false;
                }

                return true;
            }
        }

        if (order is StopOrder stopOrder)
        {
            lock (_sync)
            {
                var removed = RemoveStopOrder(stopOrder);
                if (!removed)
                {
                    return false;
                }

                return true;
                
            }
        }

        return true;
    }
    
    public IReadOnlyList<PriceLevel> RemoveStopBidsUntillPrice(Price price)
    {
        lock (_sync)
        {
            return _stopBids.RemovePriceLevelsTill(price);
        }
    }

    public IReadOnlyList<PriceLevel> RemoveStopAsksUntillPrice(Price price)
    {
        lock (_sync)
        {
            return _stopAsks.RemovePriceLevelsTill(price);
        }
    }

    private bool RemoveStopOrder(StopOrder order)
    {
        lock (_sync)
        {
            var removed = false;
            if (order.IsBuy)
            {
                removed = _stopBids.RemoveOrder(order, order.StopPrice);
                return removed;
            }

            removed = _stopAsks.RemoveOrder(order, order.StopPrice);
            return removed;
        }
    }
    private bool RemoveMarketOrder(MarketOrder order)
    {
        lock (_sync)
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
    }
    public bool AddOrder(IOrder order )
    {
        lock (_sync)
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
            if (order is StopOrder stopOrder)
            {
                try
                {
                    AddStopOrder(stopOrder);
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        return false;
    }
    public IOrder? GetBestOffer(bool isBuy, string userId)
    {
        return isBuy
            ? _bids.BestPriceLevel?.FirstOrDefault(x => x.UserId != userId)
            : _asks.BestPriceLevel?.FirstOrDefault(x => x.UserId != userId);
    }

    public void ClearBook()
    {
        _bids.Clear();
        _asks.Clear();
        _stopAsks.Clear();
        _stopBids.Clear();
        _currentMarketPrice = null;
    }

    private void AddMarketOrder(MarketOrder order)
    {
        var side = order.IsBuy ? _bids : _asks;
        side.AddOrder(order, order.Price);
    }

    private void AddStopOrder(StopOrder order)
    {
        var side = order.IsBuy ? _stopBids : _stopAsks;
        side.AddOrder(order, order.StopPrice);
    }

    public bool FillOrder(IOrder order, Quantity quantity, Price price)
    {
        lock (_sync)
        {
            var side = order.IsBuy ? _bids : _asks;
            _currentMarketPrice = price;
            return side.FillOrder(order, quantity, price);
        }
    }
    public bool CheckCanFillMarketOrderAmount(bool isBuy, Amount orderAmount)
    {
        lock (_sync)
        {
            var side = isBuy ? _bids : _asks;
            return side.CheckMarketOrderAmountCanBeFilled(orderAmount);
        }
    }
}