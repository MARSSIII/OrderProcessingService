using System.ComponentModel.DataAnnotations;

namespace Gateway.Models.Requests;

public sealed class CreateOrderRequest
{
    [Required]
    [MinLength(1)]
    public required string CreatedBy { get; init; }
}