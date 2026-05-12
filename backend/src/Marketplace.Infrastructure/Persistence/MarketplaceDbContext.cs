using Marketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure.Persistence;

public sealed class MarketplaceDbContext : DbContext
{
    public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<ExchangeRateSnapshot> ExchangeRateSnapshots => Set<ExchangeRateSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("teachers");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
            entity.Property(t => t.Level).HasColumnName("level").HasConversion<short>().IsRequired();
            entity.Property(t => t.BasePriceUsd).HasColumnName("base_price_usd").HasPrecision(10, 2).IsRequired();
            entity.HasIndex(t => t.Level).HasDatabaseName("idx_teachers_level");
        });

        modelBuilder.Entity<ExchangeRateSnapshot>(entity =>
        {
            entity.ToTable("exchange_rate_snapshots");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.BaseCurrency).HasColumnName("base_currency").HasMaxLength(3).IsRequired();
            entity.Property(x => x.TargetCurrency).HasColumnName("target_currency").HasMaxLength(3).IsRequired();
            entity.Property(x => x.Rate).HasColumnName("rate").HasPrecision(18, 6).IsRequired();
            entity.Property(x => x.SnapshotDate).HasColumnName("snapshot_date").IsRequired();
            entity.Property(x => x.FetchedAtUtc).HasColumnName("fetched_at_utc").IsRequired();

            entity.HasIndex(x => new { x.BaseCurrency, x.TargetCurrency, x.SnapshotDate })
                .IsUnique()
                .HasDatabaseName("ux_rates_base_target_snapshot_date");
            entity.HasIndex(x => new { x.BaseCurrency, x.TargetCurrency, x.SnapshotDate })
                .HasDatabaseName("idx_rates_lookup");
        });
    }
}
