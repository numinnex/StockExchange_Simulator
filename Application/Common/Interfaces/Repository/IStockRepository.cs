using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface IStockRepository : IRepository<Stock>
{
    void Update(Stock stock);
    Task SaveChangesAsync(CancellationToken token);
    Task AddRangeAsync(IEnumerable<Stock> entities);
}