using Application.Common.Models;

public interface IMatchingEngine
{
    Task<Ok<OrderCreationResult? , StopOrderProcessingResult?>> AddOrder(IOrder order, CancellationToken token);
    void CancelOrder(Guid orderId);
}