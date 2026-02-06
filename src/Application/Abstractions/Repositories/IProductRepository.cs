using Domain.Products;
using Domain.Products.Filters;

namespace Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyCollection<Product>> InsertProductAsync(IReadOnlyCollection<Product> products, CancellationToken cancellationToken);

    IAsyncEnumerable<Product> GetProductAsync(ProductFilter filter, CancellationToken cancellationToken);
}
