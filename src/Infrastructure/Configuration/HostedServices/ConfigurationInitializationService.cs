using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure.Configuration.ConfigurationsServices.Interfaces;

namespace Infrastructure.Configuration.HostedServices;

public class ConfigurationInitializationService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ConfigurationInitializationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IConfigurationUpdateService service = scope.ServiceProvider.GetRequiredService<IConfigurationUpdateService>();
        await service.OnceAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}