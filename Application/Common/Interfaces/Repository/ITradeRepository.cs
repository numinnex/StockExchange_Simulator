
using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface ITradeRepository
{
    public Task AddMarketOrderAsync(OrderMarket order, CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}