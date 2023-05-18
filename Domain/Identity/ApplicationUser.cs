using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public IList<Portfolio>? Portfolios { get; set; }
    public IList<MarketOrder>? MarketOrders { get; set; }
    public IList<StopOrder>? StopOrders { get; set; }
}