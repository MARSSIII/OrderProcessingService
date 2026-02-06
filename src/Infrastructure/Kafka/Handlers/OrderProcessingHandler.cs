using Orders.Kafka.Contracts;
using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Events;
using Infrastructure.Kafka.Abstractions;

namespace Infrastructure.Kafka.Handlers;

public sealed class OrderProcessingHandler : IMessageHandler<OrderProcessingValue>
{
    private readonly IOrderProcessingEventHandler _eventHandler;

    public OrderProcessingHandler(IOrderProcessingEventHandler eventHandler)
    {
        _eventHandler = eventHandler;
    }

    public async Task HandleAsync(
        IReadOnlyCollection<MessageEnvelope<OrderProcessingValue>> envelopes,
        CancellationToken cancellationToken)
    {
        foreach (MessageEnvelope<OrderProcessingValue> envelope in envelopes)
        {
            await ProcessSingleMessageAsync(envelope, cancellationToken);
        }
    }

    private async Task ProcessSingleMessageAsync(
        MessageEnvelope<OrderProcessingValue> envelope,
        CancellationToken cancellationToken)
    {
        OrderProcessingValue value = envelope.Payload;

        switch (value.EventCase)
        {
            case OrderProcessingValue.EventOneofCase.ApprovalReceived:
                await HandleApprovalAsync(value.ApprovalReceived, cancellationToken);
                break;

            case OrderProcessingValue.EventOneofCase.PackingStarted:
                await HandlePackingStartedAsync(value.PackingStarted, cancellationToken);
                break;

            case OrderProcessingValue.EventOneofCase.PackingFinished:
                await HandlePackingFinishedAsync(value.PackingFinished, cancellationToken);
                break;

            case OrderProcessingValue.EventOneofCase.DeliveryStarted:
                await HandleDeliveryStartedAsync(value.DeliveryStarted, cancellationToken);
                break;

            case OrderProcessingValue.EventOneofCase.DeliveryFinished:
                await HandleDeliveryFinishedAsync(value.DeliveryFinished, cancellationToken);
                break;

            case OrderProcessingValue.EventOneofCase.None:
            default:
                break;
        }
    }

    private Task HandleApprovalAsync(
        OrderProcessingValue.Types.OrderApprovalReceived e, CancellationToken cancellationToken)
    {
        return _eventHandler.HandleApprovalReceivedAsync(
            new OrderApprovalEvent(e.OrderId, e.IsApproved, e.CreatedBy, e.CreatedAt.ToDateTime()),
            cancellationToken);
    }

    private Task HandlePackingStartedAsync(
        OrderProcessingValue.Types.OrderPackingStarted e, CancellationToken cancellationToken)
    {
        return _eventHandler.HandlePackingStartedAsync(
            new OrderPackingStartedEvent(e.OrderId, e.PackingBy, e.StartedAt.ToDateTime()),
            cancellationToken);
    }

    private Task HandlePackingFinishedAsync(
        OrderProcessingValue.Types.OrderPackingFinished e, CancellationToken cancellationToken)
    {
        return _eventHandler.HandlePackingFinishedAsync(
            new OrderPackingFinishedEvent(
                e.OrderId, e.FinishedAt.ToDateTime(), e.IsFinishedSuccessfully, e.FailureReason),
            cancellationToken);
    }

    private Task HandleDeliveryStartedAsync(
        OrderProcessingValue.Types.OrderDeliveryStarted e, CancellationToken cancellationToken)
    {
        return _eventHandler.HandleDeliveryStartedAsync(
            new OrderDeliveryStartedEvent(e.OrderId, e.DeliveredBy, e.StartedAt.ToDateTime()),
            cancellationToken);
    }

    private Task HandleDeliveryFinishedAsync(
        OrderProcessingValue.Types.OrderDeliveryFinished e, CancellationToken cancellationToken)
    {
        return _eventHandler.HandleDeliveryFinishedAsync(
            new OrderDeliveryFinishedEvent(
                e.OrderId, e.FinishedAt.ToDateTime(), e.IsFinishedSuccessfully, e.FailureReason),
            cancellationToken);
    }
}