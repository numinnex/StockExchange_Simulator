
using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface ITradeRepository
{
    public Task AddMarketOrderAsync(MarketOrder order, CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
    public Task AddTradeFootprintAsync(TradeFootprint tradeFootprint, CancellationToken token);
}