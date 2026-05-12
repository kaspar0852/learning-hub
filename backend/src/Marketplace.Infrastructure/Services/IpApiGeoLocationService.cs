using System.Net.Http.Json;
using Marketplace.Application.Abstractions;
using Marketplace.Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Marketplace.Infrastructure.Services;

public sealed class IpApiGeoLocationService : IGeoLocationService
{
    private readonly HttpClient _httpClient;
    private readonly GeoIpOptions _options;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<IpApiGeoLocationService> _logger;

    public IpApiGeoLocationService(
        HttpClient httpClient,
        IOptions<GeoIpOptions> options,
        IMemoryCache memoryCache,
        ILogger<IpApiGeoLocationService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<DetectedCurrencyResult> DetectCurrencyAsync(string? ipAddress, CancellationToken cancellationToken)
    {
        var cacheKey = $"geoip:{(string.IsNullOrWhiteSpace(ipAddress) ? "unknown" : ipAddress)}";
        if (_memoryCache.TryGetValue<DetectedCurrencyResult>(cacheKey, out var cachedResult))
        {
            return cachedResult!;
        }

        DetectedCurrencyResult result;
        try
        {
            using var response = await _httpClient.GetAsync($"json/?fields=country,countryCode,currency", cancellationToken);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<IpApiResponse>(cancellationToken: cancellationToken);
            if (!string.IsNullOrWhiteSpace(payload?.Currency))
            {
                result = new DetectedCurrencyResult(
                    payload.Currency.ToUpperInvariant(),
                    payload.CountryCode,payload.CountryName);
            }
            else
            {
                result = new DetectedCurrencyResult(_options.FallbackCurrencyCode.ToUpperInvariant(), null, null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "GeoIP lookup failed. Falling back to default currency.");
            result = new DetectedCurrencyResult(_options.FallbackCurrencyCode.ToUpperInvariant(), null, null);
        }

        _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(_options.CacheMinutes));
        return result;
    }

    private sealed class IpApiResponse
    {
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
        
        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }
        
        [JsonPropertyName("country")]
        public string? CountryName { get; set; }
    }
}
