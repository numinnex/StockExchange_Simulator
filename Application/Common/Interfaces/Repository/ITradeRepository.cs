
using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface ITradeRepository
{
    public Task AddBuyTradeAsync(Trade trade, CancellationToken cancellationToken );
    public Task AddSellTradeAsync(Trade trade, CancellationToken cancellationToken);
    public Task SaveChangesAsync( CancellationToken cancellationToken ); }