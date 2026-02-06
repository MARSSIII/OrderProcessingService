using System.Globalization;
using GrpcProtos = Presentation.Protos.Products.V1;
using HttpRequests = Gateway.Models.Requests;
using HttpResponses = Gateway.Models.Responses;

namespace Gateway.Mappers;

public class ProductMapper
{
    public GrpcProtos.CreateProductsRequest ToGrpcCreateProductsRequest(HttpRequests.CreateProductsRequest request)
    {
        var grpcRequest = new GrpcProtos.CreateProductsRequest();

        foreach (HttpRequests.CreateProductItemRequest product in request.Products)
        {
            grpcRequest.Products.Add(new GrpcProtos.CreateProductItem
            {
                Name = product.Name,
                Price = product.Price.ToString(CultureInfo.CurrentCulture),
            });
        }

        return grpcRequest;
    }

    public HttpResponses.ProductListResponse ToHttpProductListResponse(GrpcProtos.ProductListResponse grpcResponse)
    {
        var products = grpcResponse.Products
            .Select(p => new HttpResponses.ProductResponse(Id: p.Id))
            .ToList();

        return new HttpResponses.ProductListResponse(Products: products);
    }
}