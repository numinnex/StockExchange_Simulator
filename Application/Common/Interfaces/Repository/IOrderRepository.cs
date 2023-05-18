using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface IOrderRepository  
{
    Task AddAsync(IOrder order, CancellationToken token);
    Task<List<MarketOrder>> GetAllMarketOrdersAsync(CancellationToken token, Expression<Func<MarketOrder, bool>>? filter = null,
        string? includeProperties = null); 
    void Update(IOrder order);
    void Remove(IOrder order);
    Task SaveChangesAsync(CancellationToken token);
}