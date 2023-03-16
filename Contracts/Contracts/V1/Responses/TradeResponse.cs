using Domain.Enums;
using Domain.ValueObjects;

namespace Contracts.V1.Responses;

public sealed class TradeResponse
{
    public required string StockId { get; init; }
    public required int Quantity { get; init; }
    public required double Price { get; init; }
    public required DateTime Timestamp { get; init; }
    public required string UserId { get; init; }
    public required TradeType Type { get; init; }
}