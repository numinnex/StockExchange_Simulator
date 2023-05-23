using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public sealed class StockRepository : Repository<Stock>, IStockRepository
{
    private readonly ApplicationDbContext _ctx;

    public StockRepository(ApplicationDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<Stock>> GetAllAsync(CancellationToken token, Expression<Func<Stock, bool>>? filter = null, string? includeProperties = null  )
    {
        IQueryable<Stock> query = _ctx.Stocks;

        if (includeProperties is not null)
        {
            foreach (var prop in includeProperties.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries))
            {
                if (prop == "TimeSeries")
                {
                    query = query.Include(x => x.TimeSeries)
                        .ThenInclude(x => x.StockValues);

                }
                else
                {
                    query = query.Include(prop);
                }
            }
        }
        
        if (filter is not null)
            query = query.Where(filter);

        return await query.ToListAsync(token);
    }

    public void UpdateAsync(Stock stock  )
    {
        _ctx.Stocks.Update( stock );
        
    }

    public async Task AddRangeAsync(List<Stock> entities, CancellationToken token)
    {
        await _ctx.Stocks.AddRangeAsync(entities,token);
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