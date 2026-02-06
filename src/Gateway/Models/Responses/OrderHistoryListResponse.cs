namespace Gateway.Models.Responses;

public record OrderHistoryListResponse(
    IReadOnlyList<OrderHistoryResponse> History);