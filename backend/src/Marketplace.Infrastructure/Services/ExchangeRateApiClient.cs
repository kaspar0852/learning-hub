using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Marketplace.Application.Exceptions;
using Marketplace.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Marketplace.Infrastructure.Services;

public sealed class ExchangeRateApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ExchangeRateOptions _options;

    public ExchangeRateApiClient(HttpClient httpClient, IOptions<ExchangeRateOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyDictionary<string, decimal>> GetLatestRatesAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new ExternalServiceAppException("ExchangeRate:ApiKey is required for scheduled sync.");
        }

        var requestPath = $"v6/{Uri.EscapeDataString(_options.ApiKey)}/latest/{Uri.EscapeDataString(_options.BaseCurrency)}";
        using var response = await _httpClient.GetAsync(requestPath, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<ExchangeRateApiResponse>(cancellationToken: cancellationToken);
        if (payload is null
            || !string.Equals(payload.Result, "success", StringComparison.OrdinalIgnoreCase)
            || payload.ConversionRates.Count == 0)
        {
            throw new ExternalServiceAppException("Exchange rate provider returned unsuccessful response.");
        }

        return payload.ConversionRates;
    }

    private sealed class ExchangeRateApiResponse
    {
        public string? Result { get; set; }
        
        [JsonPropertyName("conversion_rates")]
        public Dictionary<string, decimal> ConversionRates { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
