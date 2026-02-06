using Npgsql;
using Domain.Products;
using Infrastructure.Postgres.Mapping.Ordinals;

namespace Infrastructure.Postgres.Mapping.Mappers;

public static class ProductMapper
{
    public static Product Map(NpgsqlDataReader reader, ProductOrdinals ordinals)
    {
        return new Product(
            Id: reader.GetInt64(ordinals.Id),
            Name: reader.GetString(ordinals.Name),
            Price: reader.GetDecimal(ordinals.Price));
    }
}