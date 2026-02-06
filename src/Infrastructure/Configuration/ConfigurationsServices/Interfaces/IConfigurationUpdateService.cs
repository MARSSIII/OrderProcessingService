namespace Infrastructure.Configuration.ConfigurationsServices.Interfaces;

public interface IConfigurationUpdateService
{
    Task StartAsync(CancellationToken cancellationToken = default);

    Task OnceAsync(CancellationToken cancellationToken = default);
}
