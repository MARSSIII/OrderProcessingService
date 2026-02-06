using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;
using Application.Abstractions.Messaging;
using Infrastructure.Kafka.Abstractions;
using Infrastructure.Kafka.Options;

namespace Infrastructure.Kafka.Publishers;

public sealed class OrderEventPublisher : IOrderEventPublisher
{
    private readonly IMessageBroker _broker;
    private readonly string _orderCreationTopic;

    public OrderEventPublisher(IMessageBroker broker, IOptions<MessagingOptions> options)
    {
        _broker = broker;
        _orderCreationTopic = options.Value.Publisher.Topic;
    }

    public Task PublishOrderCreatedAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var key = new OrderCreationKey { OrderId = orderId };

        var message = new OrderCreationValue
        {
            OrderCreated = new OrderCreationValue.Types.OrderCreated
            {
                OrderId = orderId,
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            },
        };

        return _broker.SendAsync(_orderCreationTopic, key, message, cancellationToken);
    }

    public Task PublishOrderProcessingStartedAsync(long orderId, CancellationToken cancellationToken = default)
    {
        var key = new OrderCreationKey { OrderId = orderId };

        var message = new OrderCreationValue
        {
            OrderProcessingStarted = new OrderCreationValue.Types.OrderProcessingStarted
            {
                OrderId = orderId,
                StartedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            },
        };

        return _broker.SendAsync(_orderCreationTopic, key, message, cancellationToken);
    }
}