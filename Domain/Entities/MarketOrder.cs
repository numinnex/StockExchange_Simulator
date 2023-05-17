using Domain.Common;
using Domain.Enums;
using Domain.Identity;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class MarketOrder : Entity, IOrder
{
    public required bool IsBuy { get; init; }
    public required Guid StockId { get; init; }
    public Stock? Stock { get; init; }
    public required Quantity? OpenQuantity { get; set; }
    public required Amount? OrderAmount { get; set; }
    public required Price Price { get; init; }
    public Amount FeeAmount { get; set; } = 0;
    public Amount Cost { get; set; } = 0;
    public required DateTimeOffset Timestamp { get; init; }
    public required string UserId { get; init; }
    public ApplicationUser? User { get; init; }
    public TradeStatus Status { get; private set; } = TradeStatus.InQueue;
    public required TradeType Type { get; set; }
    public required TradeCondition TradeCondition { get; set; }
    public bool IsFilled => OpenQuantity == 0;
    public required string Symbol { get; init; }
    public required int FeeId { get; init; }
    public Fee? Fee { get; init; }

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