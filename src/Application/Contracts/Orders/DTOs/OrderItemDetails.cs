namespace Application.Contracts.Orders.DTOs;

public record OrderItemDetails(
    long OrderId,
    long ProductId,
    int Quantity);
