public interface IMatchingEngine
{
    Task AddOrder(IOrder order);
    void CancelOrder(Guid orderId);
}