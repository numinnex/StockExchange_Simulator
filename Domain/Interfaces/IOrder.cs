using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Domain.ValueObjects;

public interface IOrder
{
    public bool IsBuy { get; init; }
    public bool IsFilled { get; set; }
    public Guid StockId { get; init; }
    public Quantity? OpenQuantity { get; set; }
    public Amount? OrderAmount { get; set; }
    public DateTime Timestamp { get; init; }
    public string UserId { get; init; }
    public TradeType Type { get; init; }
    public TradeCondition TradeCondition { get; init; }
}