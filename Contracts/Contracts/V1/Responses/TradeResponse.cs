using Domain.Enums;
using Domain.ValueObjects;

namespace Contracts.V1.Responses;

public sealed class TradeResponse
{
    public required Guid StockId { get; init; }
    public required int Quantity { get; init; }
    public required Price Price { get; init; }
    public required DateTime Timestamp { get; init; }
    public required string UserId { get; init; }
    public required TradeStatus Status { get; init; }
    public required TradeType Type { get; init; }
    
}