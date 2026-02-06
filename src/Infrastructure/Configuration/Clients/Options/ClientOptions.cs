namespace Infrastructure.Configuration.Clients.Options;

public sealed class ClientOptions
{
    public string BaseAddress { get; set; } = string.Empty;

    public int PageSize { get; set; } = 100;
}
