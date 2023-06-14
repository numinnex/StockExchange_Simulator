using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public Guid? PortfolioId { get; init; }
    public Portfolio? Portfolio { get; set; } 
    public IList<MarketOrder>? MarketOrders { get; set; }
    public IList<StopOrder>? StopOrders { get; set; }
}