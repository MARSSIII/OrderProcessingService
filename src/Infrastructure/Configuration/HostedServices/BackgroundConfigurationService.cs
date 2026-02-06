using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure.Configuration.ConfigurationsServices.Interfaces;

namespace Infrastructure.Configuration.HostedServices;

public class BackgroundConfigurationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BackgroundConfigurationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        IConfigurationUpdateService service = scope.ServiceProvider.GetRequiredService<IConfigurationUpdateService>();
        await service.StartAsync(stoppingToken);
    }
}