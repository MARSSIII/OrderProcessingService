using Microsoft.Extensions.Options;

using Refit;

using System.Net;
using System.Runtime.CompilerServices;

using Infrastructure.Configuration.Clients.Interfaces;
using Infrastructure.Configuration.Clients.Options;
using Infrastructure.Configuration.DTOs;

namespace Infrastructure.Configuration.Clients.Refit;

public class RefitClient : IClient
{
    private readonly IRefitClient _api;

    private readonly int _pageSize;

    public RefitClient(
            IRefitClient api,
            IOptionsSnapshot<ClientOptions> options)
    {
        _api = api;
        _pageSize = options.Value.PageSize;
    }

    public async IAsyncEnumerable<ConfigurationItemDto> GetAllConfigurationsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? nextPageToken = null;

        do
        {
            QueryConfigurationsResponse<ConfigurationItemDto>? response = null;

            try
            {
                response = await _api.GetConfigurationsAsync(_pageSize, nextPageToken, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ApiException ex)
            {
                HttpStatusCode code = ex.StatusCode;

                if (code == HttpStatusCode.NotFound ||
                    code == HttpStatusCode.NoContent ||
                    code == HttpStatusCode.NotModified)
                {
                    yield break;
                }

                yield break;
            }

            if (response?.Items is null || response.Items.Count == 0)
            {
                break;
            }

            foreach (ConfigurationItemDto item in response.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return item;
            }

            nextPageToken = response.PageToken;
        }
        while (!string.IsNullOrEmpty(nextPageToken) && !cancellationToken.IsCancellationRequested);
    }
}
