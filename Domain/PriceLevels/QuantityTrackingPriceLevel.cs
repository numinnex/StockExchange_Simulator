using Domain.ValueObjects;

public class QuantityTrackingPriceLevel : IPriceLevel
{
    public QuantityTrackingPriceLevel()
    {

    }
    private readonly SortedSet<IOrder> _orders;
    public int OrderCount => _orders.Count;
    private Price _price;
    private Quantity _quantity;

    public Price Price => _price;

    public Quantity Quantity => _quantity;

    public QuantityTrackingPriceLevel(Price price)
    {
        _price = price;
        _quantity = 0;
        _orders = new SortedSet<IOrder>(OrderDateComparer.Instance);
    }

    public void AddOrder(IOrder order)
    {
        _quantity += order.OpenQuantity!;
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
}