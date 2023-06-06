using Domain.Common;
using Domain.Identity;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Portfolio : Entity
{
    public required ApplicationUser User { get; init; }
    public string UserId { get; init; }
    public List<ValueSnapshot> ValueSnapshots { get; set; } 
    public decimal TotalValue { get; set; } = 100000;
}
