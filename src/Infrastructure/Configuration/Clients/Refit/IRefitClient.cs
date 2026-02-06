using Refit;

using Infrastructure.Configuration.DTOs;

namespace Infrastructure.Configuration.Clients.Refit;

public interface IRefitClient
{
    [Get("/configurations")]
    Task<QueryConfigurationsResponse<ConfigurationItemDto>> GetConfigurationsAsync(
        [Query] int pageSize,
        [Query] string? pageToken,
        CancellationToken cancellationToken);
}
