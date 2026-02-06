using Grpc.Core;

using Application.Contracts.Products;
using Application.Contracts.Products.DTOs;

using Presentation.Api.Controllers.Mapping;
using Presentation.Protos.Products.V1;

namespace Presentation.Api.Controllers;

public sealed class ProductController : ProductGrpcService.ProductGrpcServiceBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public override async Task<ProductListResponse> CreateProductsAsync(CreateProductsRequest request, ServerCallContext context)
    {
        var createDtos = request.Products
            .Select(p => p.ToDto())
            .ToList();

        IReadOnlyCollection<ProductDetails> createdProducts = await _productService.CreateProductAsync(createDtos, context.CancellationToken);

        var response = new ProductListResponse();

        response.Products.AddRange(createdProducts.Select(p => p.ToGrpcResponse()));

        return response;
    }
}
