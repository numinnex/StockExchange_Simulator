using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.BackgroundWorkers;

public sealed class AccountValueSnapshotWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly TimeSpan _snapshotInterval;

    public AccountValueSnapshotWorker(IServiceProvider services)
    {
        _services = services;
        _snapshotInterval = TimeSpan.FromHours(24);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_snapshotInterval);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var portfolioRepository = scope.ServiceProvider.GetRequiredService<IPortfolioRepository>();
                    await TakeSnapshotAsync(portfolioRepository, stoppingToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    private async Task TakeSnapshotAsync(IPortfolioRepository portfolioRepository,CancellationToken token)
    {
        var portfolios = await portfolioRepository.GetAllAsync(token); // Get all portfolios from the repository
        if (portfolios.Count == 0)
            return;

        foreach (var portfolio in portfolios)
        {
            decimal totalValueSnapshot = portfolio.TotalValue; // Take a snapshot of the TotalValue field
            var snapshot = new ValueSnapshot
            {
                Timestamp = DateTimeOffset.UtcNow,
                Value = totalValueSnapshot
            };

            portfolio.ValueSnapshots.Add(snapshot);
        }

        await portfolioRepository.SaveChangesAsync(token); 
    }
}