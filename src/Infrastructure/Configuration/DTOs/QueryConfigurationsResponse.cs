using System.Text.Json.Serialization;

namespace Infrastructure.Configuration.DTOs;

public class QueryConfigurationsResponse<T>
{
    [JsonPropertyName("items")]
    public ICollection<T> Items { get; init; } = new List<T>();

    [JsonPropertyName("pageToken")]
    public string? PageToken { get; set; }
}
