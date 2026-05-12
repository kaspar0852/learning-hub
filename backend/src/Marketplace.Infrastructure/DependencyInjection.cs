using Marketplace.Application.Abstractions;
using Marketplace.Application.Exceptions;
using Marketplace.Infrastructure.Persistence;
using Marketplace.Infrastructure.Options;
using Marketplace.Infrastructure.Services;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Marketplace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString("Postgres")
            ?? throw new ValidationAppException("ConnectionStrings:Postgres is required.");

        services.Configure<DynamoDbOptions>(configuration.GetSection(DynamoDbOptions.SectionName));
        services.Configure<GeoIpOptions>(configuration.GetSection(GeoIpOptions.SectionName));
        services.Configure<ExchangeRateOptions>(configuration.GetSection(ExchangeRateOptions.SectionName));
        services.AddMemoryCache();
        services.AddDbContext<MarketplaceDbContext>(options => options.UseNpgsql(postgresConnection));

        services.AddSingleton<IAmazonDynamoDB>(_ =>
        {
            var options = configuration.GetSection(DynamoDbOptions.SectionName).Get<DynamoDbOptions>() ?? new DynamoDbOptions();
            var clientConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = options.ServiceUrl,
                AuthenticationRegion = options.Region,
                UseHttp = options.ServiceUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            };

            return new AmazonDynamoDBClient(new BasicAWSCredentials("local", "local"), clientConfig);
        });

        services.AddScoped<ITeacherRepository, PostgresTeacherRepository>();
        services.AddScoped<IFavoritesRepository, DynamoDbFavoritesRepository>();
        services.AddHttpClient<IGeoLocationService, IpApiGeoLocationService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<GeoIpOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });
        services.AddHttpClient<ExchangeRateApiClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ExchangeRateOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });
        services.AddScoped<IExchangeRateSyncService, ExchangeRateSyncService>();
        services.AddScoped<ICurrencyConverter, DatabaseCurrencyConverter>();
        services.AddHostedService<DatabaseInitializerHostedService>();
        services.AddHostedService<DailyExchangeRateSyncHostedService>();
        return services;
    }
}
