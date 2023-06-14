using Domain.Common;
using Domain.Identity;

namespace Domain.Entities;

public sealed class Portfolio : Entity
{
    public required string UserId { get; init; }
    public ApplicationUser? User { get; init; }
    public List<ValueSnapshot> ValueSnapshots { get; set; }
    public List<Security> Securities { get; set; }
    public decimal TotalValue { get; set; } 
}
