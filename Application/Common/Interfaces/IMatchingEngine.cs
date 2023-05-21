using Application.Common.Models;

public interface IMatchingEngine
{
    Task<Ok<OrderProcessingResult? , StopOrderProcessingResult?>> AddOrder(IOrder order, CancellationToken token);
    void CancelOrder(Guid orderId);
}