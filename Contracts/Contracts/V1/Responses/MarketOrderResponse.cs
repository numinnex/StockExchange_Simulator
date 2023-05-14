namespace Contracts.V1.Responses;

public sealed class MarketOrderResponse
{
    public required string StockId { get; init; }
    public required double Price { get; init; }
}