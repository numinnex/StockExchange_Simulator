using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface IOrderRepository  
{
    Task AddAsync(IOrder order, CancellationToken token);
    void AddSeedMarketOrdersAsync(List<MarketOrder> orders );
    Task<List<MarketOrder>> GetAllMarketOrdersAsync(CancellationToken token, Expression<Func<MarketOrder, bool>>? filter = null,
        string? includeProperties = null);
    Task<List<StopOrder>> GetAllStopOrdersAsync(CancellationToken token,Expression<Func<StopOrder, bool>>? filter = null, string? includeProperties = null);
    Task<List<MarketOrder>> GetPaginatedActiveMarketOrdersAsync( int pageNumber, int pageSize, CancellationToken token);
    Task Update(IOrder order, CancellationToken token);
    Task Remove(IOrder order, CancellationToken token);
    Task SaveChangesAsync(CancellationToken token);
}