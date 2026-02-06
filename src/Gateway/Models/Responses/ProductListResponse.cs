namespace Gateway.Models.Responses;

public record ProductListResponse(
    IReadOnlyList<ProductResponse> Products);