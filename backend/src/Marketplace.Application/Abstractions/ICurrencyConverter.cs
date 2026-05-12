namespace Marketplace.Application.Abstractions;

public interface ICurrencyConverter
{
    Task<decimal> GetUsdToCurrencyRateAsync(string currencyCode, CancellationToken cancellationToken);
}
