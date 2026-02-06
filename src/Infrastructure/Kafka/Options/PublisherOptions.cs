namespace Infrastructure.Kafka.Options;

public sealed class PublisherOptions
{
    public int TimeoutMs { get; set; } = 5000;

    public bool Idempotent { get; set; } = true;

    public string? ClientId { get; set; }

    public string Topic { get; set; } = string.Empty;
}