using System.ComponentModel.DataAnnotations;

namespace Gateway.Models.Requests;

public sealed class AddItemsToOrderRequest
{
    [Required]
    [MinLength(1)]
    public required IReadOnlyList<OrderItemCreateRequest> Items { get; init; }
}
