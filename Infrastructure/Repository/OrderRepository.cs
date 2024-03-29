using System.Linq.Expressions;
using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public sealed class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _ctx;
    private readonly DbSet<MarketOrder> _marketOrder;
    private readonly DbSet<StopOrder> _stopOrder;

    public OrderRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
        _marketOrder = _ctx.MarketOrders;
        _stopOrder = _ctx.StopOrders; 
    }
    public async Task AddAsync(IOrder order, CancellationToken token)
    {
        await using var transaction = await _ctx.Database.BeginTransactionAsync(token);
        try
        {
            if (order is MarketOrder marketOrder)
            {
                await _marketOrder.AddAsync(marketOrder, token);
            }

            if (order is StopOrder stopOrder)
            {
                await _stopOrder.AddAsync(stopOrder, token);
            }
            await SaveChangesAsync(token);
            await transaction.CommitAsync(token);
        }
        catch
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    public void AddSeedMarketOrdersAsync(List<MarketOrder> orders)
    {
        _marketOrder.AddRange(orders);
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

    public async Task<List<StopOrder>> GetAllStopOrdersAsync(CancellationToken token, Expression<Func<StopOrder, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<StopOrder> query = _stopOrder;
        if (includeProperties is not null)
        {
            foreach (var prop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(prop);
            }
        }
        if (filter is not null)
            query = query.Where(filter);
        
        return await query.ToListAsync(token);
    }
    public async Task<List<MarketOrder>> GetPaginatedActiveMarketOrdersAsync(int pageNumber, int pageSize, CancellationToken token)
    {
        var query = _ctx.MarketOrders.Where(x => x.Status == TradeStatus.InQueue).AsQueryable();

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        int skip = (pageNumber - 1) * pageSize; 
        
        var marketTrades = await query.Skip(skip).Take(pageSize).ToListAsync();
        return marketTrades;
    }

    public async Task Update(IOrder order, CancellationToken token)
    {
        await using var transaction = await _ctx.Database.BeginTransactionAsync(token);
        try
        {
            if (order is MarketOrder marketOrder)
            {
                _marketOrder.Update(marketOrder );
            }

            if (order is StopOrder stopOrder)
            {
                _stopOrder.Update(stopOrder );
            }
            await SaveChangesAsync(token);
            await transaction.CommitAsync(token);
        }
        catch
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    public async Task Remove(IOrder order, CancellationToken token)
    {
        await using var transaction = await _ctx.Database.BeginTransactionAsync(token);
        try
        {
            if (order is MarketOrder marketOrder)
            {
                _marketOrder.Remove(marketOrder);
            }

            if (order is StopOrder stopOrder)
            {
                _stopOrder.Remove(stopOrder);
            }

            await SaveChangesAsync(token);
            await transaction.CommitAsync(token);
        }
        catch
        {
            await transaction.RollbackAsync(token);
            throw;
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