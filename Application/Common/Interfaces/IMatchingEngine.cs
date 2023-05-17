using Application.Common.Models;

public interface IMatchingEngine
{
    Task<OrderCreationResult> AddOrder(IOrder order, CancellationToken token);
    void CancelOrder(Guid orderId);
}