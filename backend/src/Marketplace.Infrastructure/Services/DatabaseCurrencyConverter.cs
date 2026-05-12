using Marketplace.Application.Abstractions;
using Marketplace.Application.Exceptions;
using Marketplace.Infrastructure.Options;
using Marketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Marketplace.Infrastructure.Services;

public sealed class DatabaseCurrencyConverter : ICurrencyConverter
{
    private static readonly SemaphoreSlim BootstrapSyncLock = new(1, 1);
    private readonly MarketplaceDbContext _dbContext;
    private readonly ExchangeRateOptions _options;
    private readonly IExchangeRateSyncService _exchangeRateSyncService;

    public DatabaseCurrencyConverter(
        MarketplaceDbContext dbContext,
        IExchangeRateSyncService exchangeRateSyncService,
        IOptions<ExchangeRateOptions> options)
    {
        _dbContext = dbContext;
        _exchangeRateSyncService = exchangeRateSyncService;
        _options = options.Value;
    }

    public async Task<decimal> GetUsdToCurrencyRateAsync(string currencyCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new ValidationAppException("currency is required.");
        }

        var targetCurrency = currencyCode.Trim().ToUpperInvariant();
        if (targetCurrency == _options.BaseCurrency.ToUpperInvariant())
        {
            return 1m;
        }

        var latestRate = await _dbContext.ExchangeRateSnapshots
            .AsNoTracking()
            .Where(x => x.BaseCurrency == _options.BaseCurrency && x.TargetCurrency == targetCurrency)
            .OrderByDescending(x => x.SnapshotDate)
            .ThenByDescending(x => x.FetchedAtUtc)
            .Select(x => (decimal?)x.Rate)
            .FirstOrDefaultAsync(cancellationToken);

        if (!latestRate.HasValue)
        {
            await BootstrapSyncLock.WaitAsync(cancellationToken);
            try
            {
                await _exchangeRateSyncService.SyncLatestRatesAsync(cancellationToken);
            }
            finally
            {
                BootstrapSyncLock.Release();
            }

            latestRate = await _dbContext.ExchangeRateSnapshots
                .AsNoTracking()
                .Where(x => x.BaseCurrency == _options.BaseCurrency && x.TargetCurrency == targetCurrency)
                .OrderByDescending(x => x.SnapshotDate)
                .ThenByDescending(x => x.FetchedAtUtc)
                .Select(x => (decimal?)x.Rate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (!latestRate.HasValue)
        {
            throw new ValidationAppException($"Unsupported currency '{targetCurrency}'.");
        }

        return latestRate.Value;
    }
}
