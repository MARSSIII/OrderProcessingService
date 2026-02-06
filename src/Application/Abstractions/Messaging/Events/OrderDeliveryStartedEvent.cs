namespace Application.Abstractions.Messaging.Events;

public record OrderDeliveryStartedEvent(
    long OrderId,
    string DeliveredBy,
    DateTime StartedAt);