using Domain.Enums;

namespace Contracts.V1.Responses;

public sealed class UserTradeResponse
{
    public required decimal Quantity { get; init; }
    public required decimal OrderAmount { get; init; }
    public required decimal Price { get; init; }
    public required string Type { get; init; }
    public required bool IsBuy { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string Symbol { get; init;}
    
}