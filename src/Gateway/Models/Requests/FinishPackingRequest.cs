using System.ComponentModel.DataAnnotations;

namespace Gateway.Models.Requests;

public sealed class FinishPackingRequest
{
    [Required]
    public required bool IsSuccessful { get; init; }

    public string? FailureReason { get; init; }
}