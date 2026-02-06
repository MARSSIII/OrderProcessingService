namespace Domain.Orders.Payloads;

public record OrderCreatedByPayload(string OrderCreatedBy) : PayloadBase;
