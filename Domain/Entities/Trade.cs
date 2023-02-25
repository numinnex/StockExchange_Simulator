using Domain.Common;
using Domain.Enums;
using Domain.Identity;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Trade : Entity
{
    public required Stock Stock { get; init; }
    public required int Quantity { get; init; }
    public required Price Price { get; init; }
    public required DateTime Timestamp { get; init; }
    public required ApplicationUser User { get; init; }
    public required TradeType Type { get; init; }
}