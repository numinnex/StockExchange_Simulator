using System.Linq.Expressions;
using Application.Common.Interfaces.Repository;
using Infrastructure.Database;

namespace Infrastructure.Repository;

public sealed class OrderRepository : IOrderRepository
{
    public Task AddAsync(IOrder order, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<List<IOrder>> GetAllAsync(CancellationToken token, Expression<Func<IOrder, bool>>? filter = null, string? includeProperties = null)
    {
        throw new NotImplementedException();
    }

    public void Update(IOrder order)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }
}