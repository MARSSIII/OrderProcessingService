using System.Globalization;

using Application.Contracts.Products.DTOs;

using Presentation.Protos.Products.V1;

namespace Presentation.Api.Controllers.Mapping;

public static class ProductMappingExtensions
{
    public static CreateProductDto ToDto(this CreateProductItem item)
    {
        decimal price = decimal.Parse(item.Price, NumberStyles.Any, CultureInfo.InvariantCulture);

        return new CreateProductDto(item.Name, price);
    }

    public static ProductResponse ToGrpcResponse(this ProductDetails product)
    {
        return new ProductResponse
        {
            Id = product.Id,
        };
    }
}
