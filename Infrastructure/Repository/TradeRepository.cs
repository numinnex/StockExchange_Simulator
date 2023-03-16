using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public sealed class TradeRepository  : ITradeRepository
{
    private readonly ApplicationDbContext _ctx;

    public TradeRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }
    public async Task CreateBuyTradeAsync(Trade trade, CancellationToken cancellationToken)
    {
        var response = await _ctx.Trades.AddAsync(trade, cancellationToken);
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
}