using Microsoft.Extensions.Options;

using System.Net;
using System.Net.Http.Json;

using System.Runtime.CompilerServices;

using Infrastructure.Configuration.Clients.Interfaces;
using Infrastructure.Configuration.Clients.Options;
using Infrastructure.Configuration.DTOs;

namespace Infrastructure.Configuration.Clients.Manual;

public class ManualClient : IClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly int _pageSize;

    public ManualClient(
            IHttpClientFactory httpClientFactory,
            IOptionsSnapshot<ClientOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _pageSize = options.Value.PageSize;
    }

    public async IAsyncEnumerable<ConfigurationItemDto> GetAllConfigurationsAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? nextPageToken = null;

        do
        {
            string requestUri = BuildRequestUri(_pageSize, nextPageToken);

            QueryConfigurationsResponse<ConfigurationItemDto>? response = null;

            try
            {
                HttpClient client = _httpClientFactory.CreateClient("ManualClient");

                response = await client.GetFromJsonAsync<QueryConfigurationsResponse<ConfigurationItemDto>>(
                        requestUri,
                        cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                HttpStatusCode? code = ex.StatusCode;

                if (code == HttpStatusCode.NotFound ||
                    code == HttpStatusCode.NoContent ||
                    code == HttpStatusCode.NotModified)
                {
                    string codeText = code.ToString() ?? "unknown";

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

    private string BuildRequestUri(int pageSize, string? nextPageToken)
    {
        string requestUri = $"/configurations?pageSize={pageSize}";

        if (!string.IsNullOrEmpty(nextPageToken))
        {
            requestUri += $"&pageToken={nextPageToken}";
        }

        return requestUri;
    }
}
