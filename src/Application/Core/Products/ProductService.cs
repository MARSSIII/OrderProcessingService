using Application.Abstractions.Repositories;

using Application.Contracts.Products;
using Application.Contracts.Products.DTOs;
using Application.Mapping;
using Domain.Products;

namespace Application.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyCollection<ProductDetails>> CreateProductAsync(
            IReadOnlyCollection<CreateProductDto> createProductDtos,
            CancellationToken cancellationToken)
    {
        if (createProductDtos.Count == 0)
        {
            return Array.Empty<ProductDetails>();
        }

        var productsToInsert = createProductDtos
            .Select(dto => Product.Create(name: dto.Name, price: dto.Price))
            .ToList();

        IReadOnlyCollection<Product> inserted = await _productRepository
            .InsertProductAsync(productsToInsert, cancellationToken);

        return inserted.ToDetails();
    }
}
