namespace Gateway.Models.Responses;

public record OrderItemListResponse(
    IReadOnlyList<OrderItemResponse> Items);