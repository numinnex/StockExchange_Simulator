using Domain.Common;
using Domain.Enums;
using Domain.Identity;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class StopOrder : Entity, IOrder
{
    public required bool IsBuy { get; init; }
    public bool IsFilled => OpenQuantity == 0;
    public required bool IsTriggered { get; set; } = false;
    public required Guid StockId { get; init; }
    public Stock? Stock { get; init; }
    public required Price StopPrice { get; init; }
    public required Quantity? OpenQuantity { get; set; }
    public Amount FeeAmount { get; set; } = 0;
    public Amount Cost { get; set; } = 0;
    public required int FeeId { get; init; }
    public Fee? Fee { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string UserId { get; init; }
    public ApplicationUser? User { get; init; }
    public required string Symbol { get; init; }
    public required TradeType Type { get; init; }
    public TradeStatus Status { get; private set; } = TradeStatus.InQueue;
    
    public void UpdateTradeStatus()
    {
        if (IsFilled)
            Status = TradeStatus.Filled;
        else
            Status = TradeStatus.InQueue;
    }
    public void TradeStatusFailed()
    {
        Status = TradeStatus.Failed;
    }
}