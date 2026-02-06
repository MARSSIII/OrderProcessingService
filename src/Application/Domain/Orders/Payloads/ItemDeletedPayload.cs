namespace Domain.Orders.Payloads;

public record ItemDeletedPayload(long ProductId) : PayloadBase;
