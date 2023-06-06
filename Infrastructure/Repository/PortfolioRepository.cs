using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public sealed class PortfolioRepository : Repository<Portfolio>, IPortfolioRepository
{
    private readonly ApplicationDbContext _ctx;

    public PortfolioRepository(ApplicationDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }

    public void RemoveSecurity(Security security)
    {
        _ctx.Securities.Remove(security);
    }

    public async Task<List<Security>> GetAllUserSecuritiesAsync(string userId, CancellationToken token)
    {
        var result = await _ctx.Securities.Where(x => x.UserId == userId).ToListAsync(token);
        return result;
    }

    public async Task<List<Security>> GetSecuritiesByUserIdAsync(string userId, CancellationToken token)
    {
        var result = await _ctx.Securities.Where(x => x.UserId == userId).ToListAsync(cancellationToken: token);
        return result;
    }

    public async Task<Security?> GetSecurityByUserIdAndStockId(string stockId, string userId, CancellationToken token)
    {
        return await _ctx.Securities.FirstOrDefaultAsync(x => x.UserId == userId && x.StockId == Guid.Parse(stockId ), token);
    }

    public async Task AddSecurityAsync(Security security, CancellationToken token)
    {
        await _ctx.Securities.AddAsync(security, token);
    }

    public void UpdateSecurity(Security security )
    {
        _ctx.Securities.Update(security );
    }

    public async Task<List<Portfolio>> GetAllAsync(CancellationToken token)
    {
        return await _ctx.Portfolios.ToListAsync(token);
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