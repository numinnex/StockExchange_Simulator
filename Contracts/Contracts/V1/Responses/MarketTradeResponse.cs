namespace Contracts.V1.Responses;

public sealed class MarketTradeResponse
{
    public required string StockId { get; init; }
    public required decimal Price { get; init; }
}