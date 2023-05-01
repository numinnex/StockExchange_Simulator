using Domain.ValueObjects;

public class Side<T> where T : class, IPriceLevel, new()
{
    private readonly SortedSet<T> _priceLevels;
    public int PriceLevelCount => _priceLevels.Count;
    public IEnumerable<T> PriceLevels => _priceLevels;

    public Side()
    {
        _priceLevels = new SortedSet<T>(PriceLevelComparer.Instance);
    }
    public void AddOrder(IOrder order, Price price)
    {
        var priceLevel = GetOrAddPriceLevel(price);
        priceLevel.AddOrder(order);
    }
    public bool RemoveOrder(IOrder order, Price price)
    {
        bool removed = false;
        var priceLevelForRemoval = new T();
        priceLevelForRemoval.SetPrice(price);

        if (!_priceLevels.TryGetValue(priceLevelForRemoval, out T? priceLevel))
        {
            removed = priceLevel!.RemoveOrder(order);
            RemovePriceLevelIfEmpty(priceLevel);
        }
        return removed;
    }
    public bool FillOrder(IOrder order, Quantity quantity, Price price)
    {
        var priceLevelForSearch = new T();
        priceLevelForSearch.SetPrice(price);
        _priceLevels.TryGetValue(priceLevelForSearch, out T? priceLevel);
        bool orderFilled = priceLevel!.Fill(order, quantity);
        RemovePriceLevelIfEmpty(priceLevel);
        return orderFilled;
    }
    public bool CheckMarketOrderAmountCanBeFilled(Quantity orderAmount)
    {
        Amount cummulativeOrderAmount = 0;
        foreach (var priceLevel in _priceLevels)
        {
            if (cummulativeOrderAmount > orderAmount)
            {
                break;
            }

            cummulativeOrderAmount += (priceLevel.Quantity.Value * priceLevel.Price.Value);
        }

        return cummulativeOrderAmount >= orderAmount;
    }

    private void RemovePriceLevelIfEmpty(T priceLevel)
    {
        if (priceLevel.OrderCount == 0)
        {
            _priceLevels.Remove(priceLevel);
        }
    }

    private T GetOrAddPriceLevel(Price price)
    {
        var priceLevelForSearch = new T();
        priceLevelForSearch.SetPrice(price);
        if (_priceLevels.TryGetValue(priceLevelForSearch, out T? priceLevel))
        {
            return priceLevel;
        }
        return new T();
    }
}