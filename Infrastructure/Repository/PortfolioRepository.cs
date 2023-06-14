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
    public async Task<Portfolio?> GetByUserIdAsync(string userId, CancellationToken token)
    {
        return await _ctx.Portfolios.Include(x => x.ValueSnapshots).Include(x => x.Securities)
            .FirstOrDefaultAsync(x => x.UserId == userId, token);
    }
    public void RemoveSecurity(Security security)
    {
        _ctx.Securities.Remove(security);
    }
    public async Task AddSecurityAsync(Security security, CancellationToken token)
    {
        await _ctx.Securities.AddAsync(security, token);
    }

    public void AddRangeSecurities(List<Security> securities)
    {
        _ctx.Securities.AddRange(securities);
    }

    public async Task AddValueSnapshots(List<ValueSnapshot> values, CancellationToken token)
    {
        await _ctx.ValueSnapshots.AddRangeAsync(values, token);
    }

    public void UpdateSecurity(Security security )
    {
        _ctx.Securities.Update(security);
    }

    public void UpdatePortfolio(Portfolio portfolio)
    {
        _ctx.Portfolios.Update(portfolio);
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