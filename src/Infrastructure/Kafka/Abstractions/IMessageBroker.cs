namespace Infrastructure.Kafka.Abstractions;

public interface IMessageBroker
{
    Task SendAsync<TKey, TMessage>(
        string topic,
        TKey key,
        TMessage message,
        CancellationToken ct = default)
        where TKey : class
        where TMessage : class;
}