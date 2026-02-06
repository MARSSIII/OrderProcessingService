namespace Application.Abstractions.Messaging.Events;

public record OrderDeliveryFinishedEvent(
    long OrderId,
    DateTime FinishedAt,
    bool IsSuccessful,
    string? FailureReason);