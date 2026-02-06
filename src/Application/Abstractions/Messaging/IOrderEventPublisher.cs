namespace Application.Abstractions.Messaging;

public interface IOrderEventPublisher
{
    Task PublishOrderCreatedAsync(long orderId, CancellationToken cancellationToken = default);

    Task PublishOrderProcessingStartedAsync(long orderId, CancellationToken cancellationToken = default);
}