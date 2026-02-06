using Application.Contracts.Products.DTOs;

namespace Application.Contracts.Products;

public interface IProductService
{
    Task<IReadOnlyCollection<ProductDetails>> CreateProductAsync(
            IReadOnlyCollection<CreateProductDto> createProductDtos,
            CancellationToken cancellationToken);
}
