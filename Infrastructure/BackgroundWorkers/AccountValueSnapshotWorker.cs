using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundWorkers;

public sealed class AccountValueSnapshotWorker : BackgroundService
{
    private readonly ILogger<AccountValueSnapshotWorker> _logger;
    private readonly IServiceProvider _services;
    private readonly TimeSpan _snapshotInterval;

    public AccountValueSnapshotWorker(ILogger<AccountValueSnapshotWorker> logger ,IServiceProvider services)
    {
        _logger = logger;
        _services = services;
        _snapshotInterval = TimeSpan.FromDays(1);
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
                     _logger.LogInformation("UPDATING ACCOUNT VALUE SNAPSHOT");
                     await TakeSnapshotAsync(portfolioRepository, stoppingToken);
                     _logger.LogInformation("FINISHED UPDATING ACCOUNT VALUE SNAPSHOT");
                 }
                 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    private static async Task TakeSnapshotAsync(IPortfolioRepository portfolioRepository,CancellationToken token)
    {
        var portfolios = await portfolioRepository.GetAllAsync(token); // Get all portfolios from the repository
        if (portfolios.Count == 0)
            return;
        var valueSnapshots = new List<ValueSnapshot>();
        foreach (var portfolio in portfolios)
        {
            decimal totalValueSnapshot = portfolio.TotalValue; // Take a snapshot of the TotalValue field
            var snapshot = new ValueSnapshot
            {
                Portfolio = portfolio,
                Timestamp = DateTimeOffset.UtcNow,
                Value = totalValueSnapshot
            };
            
            valueSnapshots.Add(snapshot);
        }
        await portfolioRepository.AddValueSnapshots(valueSnapshots , token);
        await portfolioRepository.SaveChangesAsync(token);
    }
}