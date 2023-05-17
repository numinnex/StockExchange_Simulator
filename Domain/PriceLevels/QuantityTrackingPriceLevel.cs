using System.Collections;
using Domain.ValueObjects;

public class QuantityTrackingPriceLevel : IPriceLevel, IEnumerable<IOrder>
{
    private Price _price;
    private Quantity _quantity;

    private readonly SortedSet<IOrder> _orders;
    public int OrderCount => _orders.Count;
    public IOrder? First => _orders.Count > 0 ? _orders.First() : null;
    public Price Price => _price;

    public Quantity Quantity => _quantity;

    public QuantityTrackingPriceLevel()
    {
        _quantity = 0; 
        _orders = new SortedSet<IOrder>(OrderDateComparer.Instance);
    }
    public QuantityTrackingPriceLevel(Price price)
    {
        _price = price;
        _quantity = 0; 
        _orders = new SortedSet<IOrder>(OrderDateComparer.Instance);
    }

    public void AddOrder(IOrder order)
    {
        if (order.OpenQuantity is not null)
        {
            _quantity += order.OpenQuantity;
        }
        _orders.Add(order);
    }

    public bool Fill(IOrder order, Quantity quantity)
    {
        if (order.OpenQuantity! >= quantity)
        {
            _quantity -= quantity;
            order.OpenQuantity -= quantity;
            if (order.IsFilled)
            {
                return _orders.Remove(order);
            }
            return false;
        }
        return false;
    }

    public bool RemoveOrder(IOrder order)
    {
        _quantity -= order.OpenQuantity;
        return _orders.Remove(order);
    }

    public void SetPrice(Price price)
    {
        _price = price;
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