using System.ComponentModel.DataAnnotations;

namespace Gateway.Models.Requests;

public sealed class FinishDeliveryRequest
{
    [Required]
    public required bool IsSuccessful { get; init; }

    public string? FailureReason { get; init; }
}