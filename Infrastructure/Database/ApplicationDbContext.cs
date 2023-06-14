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

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
    {
        
    }

    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Security> Securities => Set<Security>();
    public DbSet<Fee> Fees => Set<Fee>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<ValueSnapshot> ValueSnapshots => Set<ValueSnapshot>();
    public DbSet<MarketOrder> MarketOrders => Set<MarketOrder>();
    public DbSet<StopOrder> StopOrders => Set<StopOrder>();
    public DbSet<StockSnapshot> StockSnapshots => Set<StockSnapshot>();
    public DbSet<TimeSeries> TimeSeries => Set<TimeSeries>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TradeFootprint> TradeFootprints => Set<TradeFootprint>();
    public DbSet<TradeDetails> TradeDetails => Set<TradeDetails>();

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

file sealed class SecurityTableConfiguration : IEntityTypeConfiguration<Security>
{
    public void Configure(EntityTypeBuilder<Security> builder)
    {
        
        builder.HasOne(x => x.Portfolio)
            .WithMany(x => x.Securities)
            .HasForeignKey(x => x.PortfolioId);
        builder.HasOne(x => x.Stock)
            .WithOne(x => x.Security)
            .HasForeignKey<Security>(x => x.StockId);

         builder.HasIndex(x => x.StockId)
             .IsUnique(false)
             .IsClustered(false);
         builder.HasIndex(x => x.OrderId)
             .IsUnique(false)
             .IsClustered(false);
            
        builder.OwnsOne(x => x.Quantity, a => a.Property(
                x => x.Value).HasColumnType("decimal(18,2)"));
        builder.Property(x => x.PurchasedPrice).HasColumnType("money");
    }
}
file sealed class StopOrderTableConfiguration : IEntityTypeConfiguration<StopOrder>
{
    public void Configure(EntityTypeBuilder<StopOrder> builder)
    {
        builder.Property(x => x.Symbol).HasMaxLength(24);
        
        builder.OwnsOne(x => x.StopPrice, a => a.Property(
                x => x.Value).HasColumnType("money"));
        builder.OwnsOne(x => x.OpenQuantity
            , a => a.Property(x => x.Value).HasColumnType("money"));
        builder.OwnsOne(x => x.FeeAmount
            , a => a.Property(x => x.Value).HasColumnType("money"));
        builder.OwnsOne(x => x.Cost
            , a => a.Property(x => x.Value).HasColumnType("money"));
        
        builder.HasOne(x => x.Stock).WithMany(x => x.StopOrders)
            .HasForeignKey(x => x.StockId);

        builder.HasOne(x => x.User).WithMany(x => x.StopOrders)
            .HasForeignKey(x => x.UserId);
    }
}

file sealed class TradeFootprintTableConfiguration : IEntityTypeConfiguration<TradeFootprint>{
    public void Configure(EntityTypeBuilder<TradeFootprint> builder)
    {
        builder.OwnsOne(x => x.Quantity, a => a.Property(
                x => x.Value).HasColumnType("decimal(18,2)"));
        builder.OwnsOne(x => x.MatchPrice, a => a.Property(
                x => x.Value).HasColumnType("money"));

        builder.Property(x => x.ProcessedOrderUserId)
            .HasMaxLength(100);
        builder.Property(x => x.RestingOrderUserId)
            .HasMaxLength(100);
    }
}

file sealed class TradeDetailsTableConfiguraion : IEntityTypeConfiguration<TradeDetails>
{
    public void Configure(EntityTypeBuilder<TradeDetails> builder)
    {
        builder.OwnsOne(x => x.AskFee, a => a.Property(
                x => x.Value).HasColumnType("money"));
        builder.OwnsOne(x => x.BidFee, a => a.Property(
                x => x.Value).HasColumnType("money"));
        builder.OwnsOne(x => x.BidCost, a => a.Property(
                x => x.Value).HasColumnType("money"));
        builder.OwnsOne(x => x.RemainingQuantity, a => a.Property(
                x => x.Value).HasColumnType("decimal"));
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
        
        builder.Property(x => x.Volume).HasColumnType("decimal(18,4)");
        builder.Property(x => x.HighMonth).HasColumnType("decimal(18,4)");
        builder.Property(x => x.LowMonth).HasColumnType("decimal(18,4)");

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
        
        builder.Property(x => x.TotalValue).HasColumnType("money");
        builder.HasOne(x => x.User).WithOne(x => x.Portfolio);
    }
}

file sealed class ValueSnapshotTableConfiguration : IEntityTypeConfiguration<ValueSnapshot>
{
    public void Configure(EntityTypeBuilder<ValueSnapshot> builder)
    {
        builder.Property(x => x.Value).HasColumnType("money");
        builder.HasOne(x => x.Portfolio)
            .WithMany(x => x.ValueSnapshots);
    }
}
file sealed class MarketOrdersTableConfiguration : IEntityTypeConfiguration<MarketOrder>
{
    public void Configure(EntityTypeBuilder<MarketOrder> builder)
    {
        builder.Property(x => x.Symbol).HasMaxLength(24);
        
        builder.OwnsOne(x => x.Price
        , a => a.Property(x => x.Value).HasColumnType("money"));

        builder.OwnsOne(x => x.OrderAmount
        , a => a.Property(x => x.Value).HasColumnType("money"));

        builder.OwnsOne(x => x.OpenQuantity
        , a => a.Property(x => x.Value).HasColumnType("money"));

        builder.OwnsOne(x => x.FeeAmount
        , a => a.Property(x => x.Value).HasColumnType("money"));

        builder.OwnsOne(x => x.Cost
        , a => a.Property(x => x.Value).HasColumnType("money"));

        builder.HasOne(x => x.Stock).WithMany(x => x.MarketOrders)
            .HasForeignKey(x => x.StockId);

        builder.HasOne(x => x.User).WithMany(x => x.MarketOrders)
            .HasForeignKey(x => x.UserId);
    }
}
file sealed class FeeTableConfiguration : IEntityTypeConfiguration<Fee>
{
    public void Configure(EntityTypeBuilder<Fee> builder)
    {
        builder.Property(x => x.MakerFee).HasPrecision(18, 2);
        builder.Property(x => x.TakerFee).HasPrecision(18, 2);
    }
}
file sealed class IdentityTableConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(x => x.Portfolio)
            .WithOne(x => x.User)
            .HasForeignKey<Portfolio>(x => x.UserId);
    }
}