using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public sealed class TradeRepository : ITradeRepository
{
    private readonly ApplicationDbContext _ctx;

    public TradeRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    //TODO -- Add transaction
    public async Task AddMarketOrderAsync(MarketOrder order, CancellationToken cancellationToken)
    {
        await _ctx.MarketOrders.AddAsync(order, cancellationToken);
    }


    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        bool saveFailed;
        do
        {
            saveFailed = false;
            try
            {
                await _ctx.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                saveFailed = true;

                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues((await entry.GetDatabaseValuesAsync(cancellationToken)));
            }

        } while (saveFailed);
    }

    public async Task AddTradeFootprintAsync(TradeFootprint tradeFootprint, CancellationToken token)
    {
        await _ctx.TradeFootprints.AddAsync(tradeFootprint, token);
    }
}