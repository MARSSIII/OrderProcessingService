namespace Application.Contracts.Orders.DTOs;

public record OrderItemsCreateDto(long ProductId, int Quantity);
