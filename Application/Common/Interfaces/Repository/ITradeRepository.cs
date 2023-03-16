
using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface ITradeRepository
{
    public Task CreateBuyTradeAsync(Trade trade, CancellationToken cancellationToken );
    public Task SaveChangesAsync( CancellationToken cancellationToken ); }