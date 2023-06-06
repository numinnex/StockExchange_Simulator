using Domain.Common;
using Domain.Identity;

namespace Domain.Entities;
public sealed class Security : SubEntity
{
    public required Guid StockId { get; init; }
    public required string UserId { get; init; }
    public ApplicationUser? User { get; init; }
    public Stock? Stock { get; init; }
    public required Quantity Quantity { get; set; }
}