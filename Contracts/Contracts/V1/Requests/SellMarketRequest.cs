namespace Contracts.V1.Requests;

public sealed class SellMarketRequest
{
    public required string StockId { get; init; }
    public required string UserId { get; init; }
    public required int Quantity { get; init; }
}