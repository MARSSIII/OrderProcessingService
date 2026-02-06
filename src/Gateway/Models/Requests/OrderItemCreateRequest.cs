namespace Gateway.Models.Requests;

public sealed class OrderItemCreateRequest
{
    public required long ProductId { get; init; }

    public required int Quantity { get; init; }
}