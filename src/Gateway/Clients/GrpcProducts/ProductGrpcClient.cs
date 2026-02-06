using Gateway.Mappers;

using GrpcProtos = Presentation.Protos.Products.V1;
using HttpRequests = Gateway.Models.Requests;
using HttpResponses = Gateway.Models.Responses;

namespace Gateway.Clients.GrpcProducts;

public class ProductGrpcClient : IProductGrpcClient
{
    private readonly GrpcProtos.ProductGrpcService.ProductGrpcServiceClient _client;

    private readonly ProductMapper _mapper;

    public ProductGrpcClient(
        GrpcProtos.ProductGrpcService.ProductGrpcServiceClient client,
        ProductMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public async Task<HttpResponses.ProductListResponse> CreateProductsAsync(
        HttpRequests.CreateProductsRequest request,
        CancellationToken cancellationToken)
    {
        GrpcProtos.CreateProductsRequest grpcRequest = _mapper.ToGrpcCreateProductsRequest(request);

        GrpcProtos.ProductListResponse response = await _client.CreateProductsAsyncAsync(grpcRequest, cancellationToken: cancellationToken);

        return _mapper.ToHttpProductListResponse(response);
    }
}