namespace Infrastructure.Kafka.Abstractions;

public sealed record MessageEnvelope<TMessage> where TMessage : class
{
    public required TMessage Payload { get; init; }

    public required string Topic { get; init; }

    public required string Key { get; init; }

    public required int Partition { get; init; }

    public required long Offset { get; init; }

    public required DateTimeOffset Timestamp { get; init; }
}