using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Marketplace.Infrastructure.Persistence;

public sealed class MarketplaceDbContextFactory : IDesignTimeDbContextFactory<MarketplaceDbContext>
{
    public MarketplaceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MarketplaceDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("MARKETPLACE_POSTGRES_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=marketplace;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);
        return new MarketplaceDbContext(optionsBuilder.Options);
    }
}
