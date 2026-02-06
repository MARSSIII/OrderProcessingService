using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure.Postgres.Extensions;

namespace Infrastructure.Postgres.HostedServices;

public class MigrationHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MigrationHostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        scope.ServiceProvider.RunMigrations();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}