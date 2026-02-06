namespace Application.Abstractions.Messaging.Events;

public record OrderPackingStartedEvent(
    long OrderId,
    string PackingBy,
    DateTime StartedAt);