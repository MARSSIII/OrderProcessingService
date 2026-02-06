namespace Infrastructure.Kafka.Abstractions;

public interface IMessageHandler<TMessage> where TMessage : class
{
    Task HandleAsync(IReadOnlyCollection<MessageEnvelope<TMessage>> envelopes, CancellationToken cancellationToken);
}