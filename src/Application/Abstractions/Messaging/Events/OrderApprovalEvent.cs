namespace Application.Abstractions.Messaging.Events;

public record OrderApprovalEvent(
    long OrderId,
    bool IsApproved,
    string CreatedBy,
    DateTime CreatedAt);