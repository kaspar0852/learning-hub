using Marketplace.Infrastructure.Options;
using Marketplace.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marketplace.Infrastructure.Persistence;

public sealed class DailyExchangeRateSyncHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ExchangeRateOptions _options;
    private readonly ILogger<DailyExchangeRateSyncHostedService> _logger;

    public DailyExchangeRateSyncHostedService(
        IServiceProvider serviceProvider,
        IOptions<ExchangeRateOptions> options,
        ILogger<DailyExchangeRateSyncHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Initial sync only if no data exists for today
        await InitialSyncIfNeededAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilNextNepalMidnight();
            await Task.Delay(delay, stoppingToken);
            await SyncOnceSafeAsync(stoppingToken);
        }
    }

    private async Task InitialSyncIfNeededAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var syncService = scope.ServiceProvider.GetRequiredService<IExchangeRateSyncService>();
            
            if (await syncService.HasTodaysRatesAsync(cancellationToken))
            {
                _logger.LogInformation("Exchange rates for today already exist. Skipping initial sync.");
                return;
            }

            await syncService.SyncLatestRatesAsync(cancellationToken);
            _logger.LogInformation("Initial exchange-rate sync completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Initial exchange-rate sync failed.");
        }
    }

    private async Task SyncOnceSafeAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SyncRatesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Daily exchange-rate sync failed.");
        }
    }

    private async Task SyncRatesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<IExchangeRateSyncService>();
        await syncService.SyncLatestRatesAsync(cancellationToken);
        _logger.LogInformation("Exchange-rate sync completed.");
    }

    private TimeSpan GetDelayUntilNextNepalMidnight()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.SyncTimeZone);
        var nowLocal = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone);
        var nextMidnightLocal = new DateTimeOffset(
                nowLocal.Year,
                nowLocal.Month,
                nowLocal.Day,
                0,
                0,
                0,
                nowLocal.Offset)
            .AddDays(1);

        var delay = nextMidnightLocal - nowLocal;
        return delay <= TimeSpan.Zero ? TimeSpan.FromMinutes(1) : delay;
    }
}