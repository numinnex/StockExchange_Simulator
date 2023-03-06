using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repository;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
    Task AddAsync(T entity);
    Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}