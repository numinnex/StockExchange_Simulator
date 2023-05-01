using System.Reflection;
using Domain.Auth;
using Domain.Entities;
using Domain.Identity;
using Infrastructure.Database.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly AuditableEntitySaveChanges _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(AuditableEntitySaveChanges auditableEntitySaveChangesInterceptor,
        DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<OrderMarket> MarketTrades => Set<OrderMarket>();
    public DbSet<StockSnapshot> StockSnapshots => Set<StockSnapshot>();
    public DbSet<TimeSeries> TimeSeries => Set<TimeSeries>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("db_stock");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //TODO - Uncomment after fixing auditableEntitySaveChangesInterceptor
        //optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}
file sealed class StockTableConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(300)
            .IsRequired();
        builder.Property(x => x.Symbol)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.Country)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.Currency)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(x => x.Change,
            a => a.Property(x => x.Value).HasColumnType("money"));
        builder.OwnsOne(x => x.Price,
            a => a.Property(x => x.Value).HasColumnType("money"));
        builder.HasOne(x => x.TimeSeries)
            .WithOne(x => x.Stock)
            .HasForeignKey<Stock>(x => x.TimeSeriesId);
    }
}

file sealed class StockSnapshotTableConfiguration : IEntityTypeConfiguration<StockSnapshot>
{
    public void Configure(EntityTypeBuilder<StockSnapshot> builder)
    {
        builder.Property(x => x.Close).HasColumnType("money");
        builder.Property(x => x.Open).HasColumnType("money");
        builder.Property(x => x.High).HasColumnType("money");
        builder.Property(x => x.Low).HasColumnType("money");

        builder.HasOne(x => x.TimeSeries)
            .WithMany(x => x.StockValues);

    }
}

file class TimeSeriesTableConfiguration : IEntityTypeConfiguration<TimeSeries>
{
    public void Configure(EntityTypeBuilder<TimeSeries> builder)
    {
        builder.Property(x => x.Interval)
            .HasMaxLength(100);
        builder.HasOne(x => x.Stock)
            .WithOne(x => x.TimeSeries);
    }
}

file sealed class PortfolioTableConfiguration : IEntityTypeConfiguration<Portfolio>
{
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(300)
            .IsRequired();
        builder.Property(x => x.TotalValue).HasColumnType("money");
        builder.OwnsMany(x => x.Positions);
    }
}

file sealed class MarketOrdersTableConfiguration : IEntityTypeConfiguration<OrderMarket>
{
    public void Configure(EntityTypeBuilder<OrderMarket> builder)
    {
        builder.OwnsOne(x => x.Price
        , a => a.Property(x => x.Value).HasColumnType("money"));

        builder.OwnsOne(x => x.OrderAmount
        , a => a.Property(x => x.Value).HasColumnType("money"));

        builder.OwnsOne(x => x.OpenQuantity
        , a => a.Property(x => x.Value).HasColumnType("decimal"));

        builder.HasOne(x => x.Stock).WithMany(x => x.MarketOrders)
            .HasForeignKey(x => x.StockId);

        builder.HasOne(x => x.User).WithMany(x => x.MarketOrders)
            .HasForeignKey(x => x.UserId);
    }
}

file sealed class IdentityTableConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasMany(x => x.Portfolios)
            .WithOne(x => x.User);
    }
}