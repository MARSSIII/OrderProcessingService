namespace Application.Abstractions.Messaging.Events;

public record OrderPackingFinishedEvent(
    long OrderId,
    DateTime FinishedAt,
    bool IsSuccessful,
    string? FailureReason);