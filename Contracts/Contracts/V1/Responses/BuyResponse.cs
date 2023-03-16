namespace Contracts.V1.Responses;

public sealed class BuyResponse
{
    public required string StockId { get; init; }
    public required int Quantity { get; init; }
}