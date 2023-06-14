using Domain.Enums;
using Domain.Identity;
using Domain.ValueObjects;

namespace Contracts.V1.Responses;

public sealed class MarketOrderResponse
{
    public required bool IsBuy { get; init; }
    public required decimal OpenQuantity { get; set; } = 0;
    public required decimal OrderAmount { get; set; } = 0;
    public required decimal Price { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string Symbol { get; init; }
}