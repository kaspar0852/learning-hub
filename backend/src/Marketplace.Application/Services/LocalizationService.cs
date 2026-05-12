using Marketplace.Application.Abstractions;

namespace Marketplace.Application.Services;

public sealed class LocalizationService
{
    private readonly IGeoLocationService _geoLocationService;

    public LocalizationService(IGeoLocationService geoLocationService)
    {
        _geoLocationService = geoLocationService;
    }

    public Task<DetectedCurrencyResult> DetectCurrencyAsync(string? ipAddress, CancellationToken cancellationToken)
    {
        return _geoLocationService.DetectCurrencyAsync(ipAddress, cancellationToken);
    }
}
