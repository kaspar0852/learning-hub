namespace Marketplace.Infrastructure.Services;

public interface IExchangeRateSyncService
{
    Task<bool> HasTodaysRatesAsync(CancellationToken cancellationToken);
    Task SyncLatestRatesAsync(CancellationToken cancellationToken);
}
