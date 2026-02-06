namespace Domain.Orders.Payloads;

public record ItemAddedPayload(long ProductId, int Quantity) : PayloadBase;
