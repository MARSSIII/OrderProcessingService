// Task1/Infrastructure/Infrastructure.Kafka/Options/SubscriptionOptions.cs

namespace Infrastructure.Kafka.Options;

public sealed class SubscriptionOptions
{
    public string Topic { get; set; } = string.Empty;

    public string GroupId { get; set; } = string.Empty;

    public string? ClientId { get; set; }

    public int ChannelCapacity { get; set; } = 1000;

    public int MaxBatchSize { get; set; } = 100;

    public int MaxBatchDelayMs { get; set; } = 1000;

    public int PollTimeoutMs { get; set; } = 50;
}