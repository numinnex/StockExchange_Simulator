namespace Contracts.V1.Requests;

public sealed class MarketOrderTradeRequest
{
    public required bool IsBuy { get; init; }
    public required string StockId { get; init; }
    public required string UserId { get; init; }
    public required decimal Quantity { get; init; }
}