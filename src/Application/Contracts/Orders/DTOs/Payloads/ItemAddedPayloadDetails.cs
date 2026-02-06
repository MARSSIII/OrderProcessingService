namespace Application.Contracts.Orders.DTOs.Payloads;

public record ItemAddedPayloadDetails(long ProductId, int Quantity) : PayloadBaseDetails;
