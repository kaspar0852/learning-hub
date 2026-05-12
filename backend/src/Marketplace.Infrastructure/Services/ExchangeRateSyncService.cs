using Marketplace.Domain.Entities;
using Marketplace.Infrastructure.Options;
using Marketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Marketplace.Infrastructure.Services;

public sealed class ExchangeRateSyncService : IExchangeRateSyncService
{
    private readonly MarketplaceDbContext _dbContext;
    private readonly ExchangeRateApiClient _exchangeRateApiClient;
    private readonly ExchangeRateOptions _options;

    public ExchangeRateSyncService(
        MarketplaceDbContext dbContext,
        ExchangeRateApiClient exchangeRateApiClient,
        IOptions<ExchangeRateOptions> options)
    {
        _dbContext = dbContext;
        _exchangeRateApiClient = exchangeRateApiClient;
        _options = options.Value;
    }

    public async Task<bool> HasTodaysRatesAsync(CancellationToken cancellationToken)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.SyncTimeZone);
        var snapshotDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone));
        var baseCurrency = _options.BaseCurrency.Trim().ToUpperInvariant();

        return await _dbContext.ExchangeRateSnapshots
            .AnyAsync(x => x.BaseCurrency == baseCurrency && x.SnapshotDate == snapshotDate, cancellationToken);
    }

    public async Task SyncLatestRatesAsync(CancellationToken cancellationToken)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.SyncTimeZone);
        var snapshotDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone));
        var fetchedAtUtc = DateTime.UtcNow;
        var baseCurrency = _options.BaseCurrency.Trim().ToUpperInvariant();

        var existingRates = await _dbContext.ExchangeRateSnapshots
            .Where(x => x.BaseCurrency == baseCurrency && x.SnapshotDate == snapshotDate)
            .ToDictionaryAsync(x => x.TargetCurrency, StringComparer.OrdinalIgnoreCase, cancellationToken);
        
        var rates = await _exchangeRateApiClient.GetLatestRatesAsync(cancellationToken);

        foreach (var pair in rates)
        {
            var targetCurrency = pair.Key.Trim().ToUpperInvariant();
            if (pair.Value <= 0)
            {
                continue;
            }

            if (existingRates.TryGetValue(targetCurrency, out var row))
            {
                row.Rate = pair.Value;
                row.FetchedAtUtc = fetchedAtUtc;
            }
            else
            {
                _dbContext.ExchangeRateSnapshots.Add(new ExchangeRateSnapshot
                {
                    Id = Guid.NewGuid(),
                    BaseCurrency = baseCurrency,
                    TargetCurrency = targetCurrency,
                    Rate = pair.Value,
                    SnapshotDate = snapshotDate,
                    FetchedAtUtc = fetchedAtUtc
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}