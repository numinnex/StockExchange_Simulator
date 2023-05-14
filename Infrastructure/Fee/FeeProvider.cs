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
    public async Task<Fee?> GetFee(int feeId)
    {
        return await _ctx.Fees.FindAsync(feeId);
    }

}