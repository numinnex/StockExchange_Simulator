using System.Reflection;
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
    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<StockSnapshot> StockSnapshots => Set<StockSnapshot>();
    public DbSet<TimeSeries> TimeSeries => Set<TimeSeries>();


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
file sealed class StockTableConfiguration : IEntityTypeConfiguration<Stock>{
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

        builder.OwnsOne(x => x.Change ,
            a => a.Property(x => x.Value).HasColumnType("money"));
        builder.OwnsOne(x => x.Price ,
            a => a.Property(x => x.Value).HasColumnType("money"));
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
        
    }
}

file class TimeSeriesTableConfiguration : IEntityTypeConfiguration<TimeSeries>
{
    public void Configure(EntityTypeBuilder<TimeSeries> builder)
    {
        builder.Property(x => x.Inteval)
            .HasMaxLength(100);
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
        builder.OwnsMany(x => x.Positions );
    }
}

file sealed class TradeTableConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        builder.OwnsOne(x => x.Price 
        , a => a.Property(x => x.Value).HasColumnType("money"));
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