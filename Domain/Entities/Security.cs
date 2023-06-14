using Domain.Common;

namespace Domain.Entities;
public sealed class Security : SubEntity
{
    public required Guid PortfolioId { get; init; }
    public Portfolio? Portfolio { get; init; }
    public Guid StockId { get; init; }
    public Stock? Stock { get; init; }
    public Guid? OrderId { get; init; }
    public MarketOrder? Order { get; init; }
    public decimal PurchasedPrice { get; set; }
    public required DateTimeOffset Timestamp { get; init; }
    public required Quantity Quantity { get; set; }
}