using Microsoft.Extensions.Options;
using Infrastructure.Configuration.Clients.Interfaces;
using Infrastructure.Configuration.ConfigurationsServices.Interfaces;
using Infrastructure.Configuration.ConfigurationsServices.Options;
using Infrastructure.Configuration.ConfigurationsServices.Provider;
using Infrastructure.Configuration.DTOs;

namespace Infrastructure.Configuration.ConfigurationsServices.Services;

public class ConfigurationUpdateService : IConfigurationUpdateService
{
    private readonly IClient _client;

    private readonly ConfigurationServiceProvider _provider;

    private readonly TimeSpan _updateInterval;

    public ConfigurationUpdateService(
        IClient client,
        ConfigurationServiceProvider provider,
        IOptionsSnapshot<ConfigurationServiceOptions> options)
    {
        _client = client;
        _provider = provider;
        _updateInterval = options.Value.UpdateInterval;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_updateInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await OnceAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async Task OnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            var configItems = new List<ConfigurationItemDto>();

            await foreach (ConfigurationItemDto item in _client.GetAllConfigurationsAsync(cancellationToken))
            {
                configItems.Add(item);
            }

            _provider.UpdateConfiguration(configItems);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        }
    }
}
