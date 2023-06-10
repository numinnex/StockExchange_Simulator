namespace Contracts.V1.Responses;

public sealed class UserSecuritiesResponse
{
    public required decimal Quantity { get; init; }
    public required decimal PurchasedPrice { get; init; }
    public required decimal CurrentPrice { get; init; }
    public required decimal TotalValue { get; init; }
    public required string Symbol { get; init; }
    
}