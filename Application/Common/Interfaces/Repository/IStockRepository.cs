using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface IStockRepository : IRepository<Stock>
{
    Task<List<Stock>> GetAllAsync( CancellationToken token,Expression<Func<Stock, bool>>? filter = null, string? includeProperties = null );
    void Update(Stock stock);
    Task SaveChangesAsync(CancellationToken token);
    Task AddRangeAsync(IEnumerable<Stock> entities, CancellationToken token);
}