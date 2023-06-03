using Application.Common.Interfaces.Repository;
using Infrastructure.Database;

namespace Infrastructure.Repository;

public sealed class FeeRepository : Repository<Fee>,IFeeRepository
{
    private readonly ApplicationDbContext _ctx;

    public FeeRepository(ApplicationDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }

    public async Task<bool> ExistAsync(int id, CancellationToken token)
    {
        return await _ctx.Fees.FindAsync(id ) is not null;
    }

    public async Task SaveChangesAsync(CancellationToken token)
    {
        await _ctx.SaveChangesAsync(token);
    }
}