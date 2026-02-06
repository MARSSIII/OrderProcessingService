namespace Gateway.Models.Responses;

public record OrderItemResponse(
    long OrderId,
    long ProductId,
    int Quantity);