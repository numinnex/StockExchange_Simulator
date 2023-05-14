public interface IMatchingEngine
{
    Task AddOrder(IOrder order, CancellationToken token);
    void CancelOrder(Guid orderId);
}