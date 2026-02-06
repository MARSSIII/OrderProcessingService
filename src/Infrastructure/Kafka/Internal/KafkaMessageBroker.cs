using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Infrastructure.Kafka.Abstractions;
using Infrastructure.Kafka.Options;

namespace Infrastructure.Kafka.Internal;

internal sealed class KafkaMessageBroker : IMessageBroker, IDisposable
{
    private readonly IProducer<byte[], byte[]> _producer;
    private readonly IMessageSerializer _serializer;

    public KafkaMessageBroker(
        IOptions<MessagingOptions> options,
        IMessageSerializer serializer)
    {
        _serializer = serializer;

        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            EnableIdempotence = options.Value.Publisher.Idempotent,
            MessageTimeoutMs = options.Value.Publisher.TimeoutMs,
            ClientId = options.Value.Publisher.ClientId,
        };

        _producer = new ProducerBuilder<byte[], byte[]>(config).Build();
    }

    public async Task SendAsync<TKey, TMessage>(
        string topic,
        TKey key,
        TMessage message,
        CancellationToken ct = default)
        where TKey : class
        where TMessage : class
    {
        byte[] keyBytes = _serializer.Serialize(key);
        byte[] valueBytes = _serializer.Serialize(message);

        var kafkaMessage = new Message<byte[], byte[]>
        {
            Key = keyBytes,
            Value = valueBytes,
        };

        await _producer.ProduceAsync(topic, kafkaMessage, ct);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}