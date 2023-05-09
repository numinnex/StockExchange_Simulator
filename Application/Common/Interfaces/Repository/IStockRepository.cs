using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface IStockRepository : IRepository<Stock>
{
    Task<List<Stock>> GetAllAsync( CancellationToken token,Expression<Func<Stock, bool>>? filter = null, string? includeProperties = null );
    void UpdateAsync(Stock stock );
    Task AddRangeAsync(IEnumerable<Stock> entities, CancellationToken token);
    Task SaveChangesAsync(CancellationToken token);
    Task<bool> ExistAsync(string id, CancellationToken token);
}