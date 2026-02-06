using System.ComponentModel.DataAnnotations;

namespace Gateway.Models.Requests;

public sealed class ApproveOrderRequest
{
    [Required]
    public required bool IsApproved { get; init; }

    [Required]
    [MinLength(1)]
    public required string ApprovedBy { get; init; }

    public string? FailureReason { get; init; }
}