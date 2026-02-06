using System.ComponentModel.DataAnnotations;

namespace Gateway.Models.Requests;

public sealed class StartPackingRequest
{
    [Required]
    [MinLength(1)]
    public required string PackingBy { get; init; }
}