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

    public void Update(Stock stock )
    {
        _ctx.Stocks.Update( stock );
    }

    public async Task SaveChangesAsync(CancellationToken token)
    {
        await _ctx.SaveChangesAsync(token);
    }

    public async Task AddRangeAsync(IEnumerable<Stock> entities, CancellationToken token)
    {
        await _ctx.Stocks.AddRangeAsync(entities,token);
    }
}