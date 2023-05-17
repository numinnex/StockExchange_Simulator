using System.Linq.Expressions;
using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public sealed class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _ctx;
    private readonly DbSet<MarketOrder> _marketOrder;

    public OrderRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
        _marketOrder = _ctx.MarketTrades;
    }
    public async Task AddAsync(IOrder order, CancellationToken token)
    {
        if (order is MarketOrder marketOrder)
        {
           await _marketOrder.AddAsync(marketOrder, token);
        }
    }

    public async Task<List<MarketOrder>> GetAllMarketOrdersAsync(CancellationToken token, Expression<Func<MarketOrder, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<MarketOrder> query = _marketOrder;
        if (includeProperties is not null)
        {
            foreach (var prop in includeProperties.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(prop);
            }
        }
        if (filter is not null)
            query = query.Where(filter);
        return await query.ToListAsync(token);
    }

    public void Update(IOrder order)
    {
        if (order is MarketOrder marketOrder)
        {
            _marketOrder.Update(marketOrder);
        }
    }

    public async Task SaveChangesAsync(CancellationToken token)
    {
        bool saveFailed;
        do
        {
            saveFailed = false;
            try
            {
                await _ctx.SaveChangesAsync(token);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                saveFailed = true;

                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues((await entry.GetDatabaseValuesAsync(token)));
            }

        } while (saveFailed);

    }
}