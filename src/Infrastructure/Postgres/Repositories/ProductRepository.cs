using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using Application.Abstractions.Repositories;
using Domain.Products;
using Domain.Products.Filters;
using Infrastructure.Postgres.Mapping.Mappers;
using Infrastructure.Postgres.Mapping.Ordinals;

namespace Infrastructure.Postgres.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ProductRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IReadOnlyCollection<Product>> InsertProductAsync(IReadOnlyCollection<Product> products, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO products(product_name, product_price)
        SELECT name, price FROM unnest(:name, :price) as source (name, price)
        RETURNING product_id;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("name", products.Select(x => x.Name).ToArray()),
                new NpgsqlParameter("price", products.Select(x => x.Price).ToArray()),
            },
        };

        var productList = products.ToList();
        var ids = new List<long>(productList.Count);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            ids.Add(reader.GetInt64(reader.GetOrdinal("product_id")));
        }

        if (ids.Count != productList.Count)
            throw new InvalidOperationException("Product insertion operation failed: ids count mismatch.");

        var result = new List<Product>(productList.Count);
        for (int i = 0; i < productList.Count; i++)
        {
            result.Add(productList[i] with { Id = ids[i] });
        }

        return result;
    }

    public async IAsyncEnumerable<Product> GetProductAsync(ProductFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT product_id, product_name, product_price
        FROM products
        WHERE
             (product_id > :cursor)
            AND(cardinality(:ids) = 0 OR product_id = ANY(:ids))
            AND(:minPrice IS NULL OR product_price >= :minPrice)
            AND(:maxPrice IS NULL OR product_price <= :maxPrice)
            AND(:subStringName IS NULL OR product_name ILIKE :subStringName)
        ORDER BY product_id
        LIMIT :pageSize;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", filter.Cursor),
                new NpgsqlParameter("ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
                {
                    Value = filter.Ids ?? [],
                },
                new NpgsqlParameter("minPrice", NpgsqlDbType.Money)
                {
                    Value = filter.MinPrice.HasValue ? filter.MinPrice.Value : DBNull.Value,
                },
                new NpgsqlParameter("maxPrice", NpgsqlDbType.Money)
                {
                    Value = filter.MaxPrice.HasValue ? filter.MaxPrice.Value : DBNull.Value,
                },
                new NpgsqlParameter("subStringName", NpgsqlDbType.Text)
                {
                    Value = string.IsNullOrWhiteSpace(filter.SubStringName) ? DBNull.Value : $"%{filter.SubStringName}%",
                },
                new NpgsqlParameter("pageSize", filter.PageSize),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        var ordinals = new ProductOrdinals(reader);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return ProductMapper.Map(reader, ordinals);
        }
    }
}
