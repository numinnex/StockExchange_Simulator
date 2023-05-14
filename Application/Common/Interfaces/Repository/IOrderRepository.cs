using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repository;

public interface IOrderRepository  
{
    Task AddAsync(IOrder order, CancellationToken token);
    Task<List<IOrder>> GetAllAsync(CancellationToken token, Expression<Func<IOrder, bool>>? filter = null,
        string? includeProperties = null); 
    void Update(IOrder order);
    Task SaveChangesAsync(CancellationToken token);
}