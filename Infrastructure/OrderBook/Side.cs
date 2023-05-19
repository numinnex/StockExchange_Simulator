using System.Collections.ObjectModel;
using Domain.ValueObjects;

public class Side<T> where T : class, IPriceLevel, new()
{
    private readonly SortedSet<T> _priceLevels;
    private T? _bestPriceLevel;
    private IComparer<Price> _priceComparer;
    public int PriceLevelCount => _priceLevels.Count;
    public IEnumerable<T> PriceLevels => _priceLevels;
    public T? BestPriceLevel => _bestPriceLevel;


    public Side(IComparer<Price> priceComparer, IComparer<T> priceLevelComparer)
    {
        _priceComparer = priceComparer;
        _priceLevels = new SortedSet<T>(priceLevelComparer);
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

        if (priceLevel is null)
            return false;

        bool orderFilled = priceLevel.Fill(order, quantity);
        RemovePriceLevelIfEmpty(priceLevel);
        return orderFilled;
    }
    public bool CheckMarketOrderAmountCanBeFilled(Amount orderAmount)
    {
        Amount cummulativeOrderAmount = 0;
        foreach (var priceLevel in _priceLevels)
        {
            if (cummulativeOrderAmount > orderAmount)
            {
                break;
            }

            cummulativeOrderAmount += (priceLevel.Quantity * priceLevel.Price);
        }

        return cummulativeOrderAmount >= orderAmount;
    }

    public IReadOnlyList<T> RemovePriceLevelsTill(Price price)
    {
        if (_bestPriceLevel is not null && _priceComparer.Compare(_bestPriceLevel.Price, price) <= 0)
        {
            List<T> priceLevels = new List<T>();
            foreach (var priceLevel in _priceLevels)
            {
                if (_priceComparer.Compare(price, priceLevel.Price) <= 0)
                {
                    priceLevels.Add(priceLevel);    
                }
                else
                {
                    _bestPriceLevel = priceLevel;
                    break;
                }
            }

            for (int i = 0; i < priceLevels.Count; i++)
            {
                _priceLevels.Remove(priceLevels[i]);
            }

            if (_priceLevels.Count == 0)
            {
                _bestPriceLevel = null;
            }

            return priceLevels.AsReadOnly();
        }

        return Enumerable.Empty<T>().ToList();
    }

    private void RemovePriceLevelIfEmpty(T priceLevel)
    {
        if (_bestPriceLevel.Equals(priceLevel))
        {
            _bestPriceLevel = _priceLevels.ElementAt(_priceLevels.Count - 1);
        }
        if (priceLevel.OrderCount == 0)
        {
            _priceLevels.Remove(priceLevel);
        }
    }

    private T GetOrAddPriceLevel(Price price)
    {
        var priceLevelForSearch = new T();
        priceLevelForSearch.SetPrice(price);
        if (!_priceLevels.TryGetValue(priceLevelForSearch, out T? priceLevel))
        {
            priceLevel = new T();
            priceLevel.SetPrice(price);
            _priceLevels.Add(priceLevel);
            if (_bestPriceLevel is null || _priceComparer.Compare(price, _bestPriceLevel.Price) < 0)
            {
                _bestPriceLevel = priceLevel;
            }
        }
        return priceLevel;
    }

    //for testing purposes only
    public void Clear()
    {
        _priceLevels.Clear();
        _bestPriceLevel = null;
    }
}