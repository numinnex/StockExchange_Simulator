using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class TradeFootprint  
{
    public int Id { get; init; }
    public required Guid StockId { get; init; }
    public required bool ProcessedOrderIsBuy { get; init; }
    public required Guid ProcessedOrderId { get; init; }     
    public required Guid RestingOrderId { get; init; }     
    public required string ProcessedOrderUserId { get; init; }     
    public required string RestingOrderUserId { get; init; }     
    public required Price MatchPrice { get; init; }
    public required Quantity Quantity { get; init; }
    public int TradeDetailsId { get; init; }
    public required TradeDetails TradeDetails { get; init; }
}
public sealed class TradeDetails
{
    public int Id { get; init; }
    public Amount? BidFee { get; init; }
    public Amount? AskFee { get; init; }
    public Amount? BidCost { get; init; }
    public Quantity? RemainingQuantity { get; init; }
}