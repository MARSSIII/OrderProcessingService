using Infrastructure.Configuration.DTOs;

namespace Infrastructure.Configuration.Clients.Interfaces;

public interface IClient
{
    IAsyncEnumerable<ConfigurationItemDto> GetAllConfigurationsAsync(CancellationToken cancellationToken = default);
}
