using Npgsql;

namespace Infrastructure.Postgres.Mapping.Ordinals;

public readonly struct ProductOrdinals
{
    public int Id { get; }

    public int Name { get; }

    public int Price { get; }

    public ProductOrdinals(NpgsqlDataReader reader)
    {
        Id = reader.GetOrdinal("product_id");
        Name = reader.GetOrdinal("product_name");
        Price = reader.GetOrdinal("product_price");
    }
}