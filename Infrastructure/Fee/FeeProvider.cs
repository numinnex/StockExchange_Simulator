using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

public class FeeProvider : IFeeProvider
{
    private readonly ApplicationDbContext _ctx;

    public FeeProvider(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }
    public async Task<Fee?> GetFeeAsync(int feeId)
    {
        return await _ctx.Fees.AsNoTracking().FirstOrDefaultAsync(f => f.Id == feeId);
        
    }

}