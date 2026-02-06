using System.ComponentModel.DataAnnotations;

namespace Gateway.Models.Requests;

public sealed class StartDeliveryRequest
{
    [Required]
    [MinLength(1)]
    public required string DeliveredBy { get; init; }
}