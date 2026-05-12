namespace Marketplace.Application.Abstractions;

public interface IGeoLocationService
{
    Task<DetectedCurrencyResult> DetectCurrencyAsync(string? ipAddress, CancellationToken cancellationToken);
}

public sealed record DetectedCurrencyResult(string CurrencyCode, string? CountryCode, string? CountryName);
