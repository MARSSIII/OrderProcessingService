using Application.Contracts.Products.DTOs;
using Domain.Products;

namespace Application.Mapping;

public static class ProductMappingExtensions
{
    public static ProductDetails ToDetails(this Product product)
    {
        return new ProductDetails(Id: product.Id);
    }

    public static IReadOnlyCollection<ProductDetails> ToDetails(
        this IReadOnlyCollection<Product> products)
    {
        return products.Select(p => p.ToDetails()).ToList();
    }
}