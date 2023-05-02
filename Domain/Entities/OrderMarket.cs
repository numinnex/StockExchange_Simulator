using Domain.Common;
using Domain.Enums;
using Domain.Identity;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class OrderMarket : Entity, IOrder
{
    public required bool IsBuy { get; init; }
    public required Guid StockId { get; init; }
    public Stock? Stock { get; init; }
    public required Quantity? OpenQuantity { get; set; }
    public required Amount? OrderAmount { get; set; }
    public required Price Price { get; init; }
    public required DateTime Timestamp { get; init; }
    public required string UserId { get; init; }
    public ApplicationUser? User { get; init; }
    public required TradeStatus Status { get; init; }
    public required TradeType Type { get; init; }
    public required TradeCondition TradeCondition { get; init; }
    public bool IsFilled => OpenQuantity == 0;
}