namespace Gateway.Models.Responses;

public record ErrorResponse(
    string Message,
    string? Details = null);