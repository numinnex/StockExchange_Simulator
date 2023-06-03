using System.Data.Common;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.MsSql;
using Xunit;

public sealed class DbContextFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer =
        new MsSqlBuilder()
            .WithCleanUp(true)
            .Build();

    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;
    public ApplicationDbContext ctx { get; private set; }
    private DbContextOptions<ApplicationDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .Options;
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        ctx = new ApplicationDbContext(CreateOptions());
        ctx.Database.Migrate();
        await InitializeRespawner();
    }

    private async Task InitializeRespawner()
    {
        _dbConnection = ctx.Database.GetDbConnection();
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions()
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = new[] { "db_stock" }
        });
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}