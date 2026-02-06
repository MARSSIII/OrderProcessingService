namespace Infrastructure.Kafka.Options;

public sealed class MessagingOptions
{
    public string BootstrapServers { get; set; } = "localhost:9092";

    public PublisherOptions Publisher { get; set; } = new();

    public Dictionary<string, SubscriptionOptions> Subscriptions { get; } = new();
}
