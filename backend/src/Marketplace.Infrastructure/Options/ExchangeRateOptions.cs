namespace Marketplace.Infrastructure.Options;

public sealed class ExchangeRateOptions
{
    public const string SectionName = "ExchangeRate";

    public string BaseUrl { get; init; } = "https://v6.exchangerate-api.com/";
    public string ApiKey { get; init; } = string.Empty;
    public int TimeoutSeconds { get; init; } = 5;
    public string BaseCurrency { get; init; } = "USD";
    public string SyncTimeZone { get; init; } = "Asia/Kathmandu";
}
