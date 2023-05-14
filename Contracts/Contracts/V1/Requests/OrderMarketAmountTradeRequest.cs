namespace Contracts.V1.Requests;

public sealed class OrderMarketAmountTradeRequest
{
    public required bool IsBuy { get; init; }
    public required string StockId { get; init; }
    public required string UserId { get; init; }
    public required decimal Amount { get; init; }
}