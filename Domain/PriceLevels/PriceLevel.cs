using System.Collections;
using Domain.ValueObjects;

namespace Domain.PriceLevels;

public sealed class PriceLevel : IPriceLevel , IEnumerable<IOrder>
{
    private Price _price;
    private readonly List<IOrder> _orders;
    public int OrderCount => _orders.Count;
    public Price Price => _price; 
    public Quantity Quantity => _orders.Sum(x => x.OpenQuantity!);

    public PriceLevel()
    {
        _orders = new();
    }

    public PriceLevel(Price price)
    {
        _price = price;
        _orders = new();
    }
    public void AddOrder(IOrder order)
    {
        _orders.Add(order);
    }

    public bool RemoveOrder(IOrder order)
    {
        return _orders.Remove(order);
    }

    public void SetPrice(Price price)
    {
        _price = price;
    }

    public bool Fill(IOrder order, Quantity quantity)
    {
        if (order.OpenQuantity! >= quantity)
        {
            order.OpenQuantity -= quantity;
            if (order.IsFilled)
            {
                return _orders.Remove(order);
            }
            return false;
        }
        return false;
    }

    public IEnumerator<IOrder> GetEnumerator()
    {
        return ((IEnumerable<IOrder>)_orders).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}