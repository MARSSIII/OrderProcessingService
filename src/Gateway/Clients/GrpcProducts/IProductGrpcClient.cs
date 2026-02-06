using Gateway.Models.Requests;
using Gateway.Models.Responses;

namespace Gateway.Clients.GrpcProducts;

public interface IProductGrpcClient
{
    Task<ProductListResponse> CreateProductsAsync(
        CreateProductsRequest request,
        CancellationToken cancellationToken);
}