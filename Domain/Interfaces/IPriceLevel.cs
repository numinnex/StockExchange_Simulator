using Domain.ValueObjects;

public interface IPriceLevel
{
    public int OrderCount { get; }
    public Price Price { get; }
    public Quantity Quantity { get; }
    void AddOrder(IOrder order);
    bool RemoveOrder(IOrder order);
    void SetPrice(Price price);
    bool Fill(IOrder order, Quantity quantity);
}