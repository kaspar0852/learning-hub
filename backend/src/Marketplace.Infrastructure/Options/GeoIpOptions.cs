namespace Marketplace.Infrastructure.Options;

public sealed class GeoIpOptions
{
    public const string SectionName = "GeoIp";

    public string BaseUrl { get; init; } = "https://ipapi.co/";
    public int TimeoutSeconds { get; init; } = 5;
    public int CacheMinutes { get; init; } = 30;
    public string FallbackCurrencyCode { get; init; } = "USD";
}
