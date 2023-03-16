namespace Contracts.V1.Responses;

public sealed class SellResponse
{
    public required string StockId { get; init; }
    public required int Quantity { get; init; }
    //public required double Price { get; init; }
}