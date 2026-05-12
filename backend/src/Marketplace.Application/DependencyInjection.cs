using Marketplace.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<MarketplaceService>();
        services.AddScoped<LocalizationService>();
        return services;
    }
}
