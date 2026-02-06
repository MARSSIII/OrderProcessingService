using System.ComponentModel.DataAnnotations;

namespace Gateway.Models.Requests;

public sealed class CreateProductItemRequest
{
    [Required]
    [MinLength(1)]
    public required string Name { get; init; }

    [Required]
    public required decimal Price { get; init; }
}