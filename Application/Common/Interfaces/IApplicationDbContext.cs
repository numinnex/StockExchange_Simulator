using Application.Common.Models.ReadModels;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Stock> Stocks { get; }
    DbSet<Portfolio> Portfolios { get; }
    DbSet<Trade> Trades { get; }
    DbSet<StockSnapshot> StockSnapshots { get; }
    DbSet<TimeSeries> TimeSeries { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
