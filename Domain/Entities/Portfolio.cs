using Domain.Common;
using Domain.Identity;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Portfolio : Entity
{
    public required ApplicationUser User { get; init; }
    public required string Name { get; set; }
    public IList<StockPosition>? Positions { get; set; }
    public double TotalValue { get; set; }
}
