using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using Infrastructure.Configuration.Clients.Interfaces;
using Infrastructure.Configuration.Clients.Manual;
using Infrastructure.Configuration.Clients.Options;
using Infrastructure.Configuration.Clients.Refit;
using Infrastructure.Configuration.ConfigurationsServices.Interfaces;
using Infrastructure.Configuration.ConfigurationsServices.Options;
using Infrastructure.Configuration.ConfigurationsServices.Provider;
using Infrastructure.Configuration.ConfigurationsServices.Services;

namespace Infrastructure.Configuration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomConfiguration(
        this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        services.AddSingleton<IConfigurationUpdateService, ConfigurationUpdateService>();

        var provider = new ConfigurationServiceProvider();

        configurationManager.Add(new ConfigurationServiceSource(provider));
        services.AddSingleton(provider);

        services.AddOptions<ConfigurationServiceOptions>().BindConfiguration("RemoteConfiguration");

        services.AddOptions<ClientOptions>().BindConfiguration("ConfigurationClient");

        return services;
    }

    public static IServiceCollection AddManualConfigurationClient(
        this IServiceCollection services)
    {
        services.AddTransient<IClient, ManualClient>();

        services.AddHttpClient("ManualClient", (serviceProvider, client) =>
        {
            ClientOptions options = serviceProvider.GetRequiredService<IOptions<ClientOptions>>().Value;

            client.BaseAddress = new Uri(options.BaseAddress);
        });

        return services;
    }

    public static IServiceCollection AddRefitConfigurationClient(
        this IServiceCollection services)
    {
        services.AddTransient<IClient, RefitClient>();

        services.AddRefitClient<IRefitClient>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                ClientOptions options = serviceProvider.GetRequiredService<IOptions<ClientOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseAddress);
            });

        return services;
    }
}