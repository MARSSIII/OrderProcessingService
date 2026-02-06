using System.ComponentModel.DataAnnotations;

namespace Gateway.Models.Requests;

public sealed class CreateProductsRequest
{
    [Required]
    [MinLength(1)]
    public required IReadOnlyList<CreateProductItemRequest> Products { get; init; }
}
