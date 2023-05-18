using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Domain.ValueObjects;

public interface IOrder
{
    public bool IsBuy { get; init; }
    public bool IsFilled { get; }
    public Guid StockId { get; init; }
    public Quantity? OpenQuantity { get; set; }
    public int FeeId { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public string UserId { get; init; }
    public string Symbol { get; init; }
    public TradeType Type { get; init; }
}