using Npgsql;

namespace Infrastructure.Postgres.Mapping.Ordinals;

public readonly struct OrderItemOrdinals
{
    public int Id { get; }

    public int OrderId { get; }

    public int ProductId { get; }

    public int Quantity { get; }

    public int IsDeleted { get; }

    public OrderItemOrdinals(NpgsqlDataReader reader)
    {
        Id = reader.GetOrdinal("order_item_id");
        OrderId = reader.GetOrdinal("order_id");
        ProductId = reader.GetOrdinal("product_id");
        Quantity = reader.GetOrdinal("order_item_quantity");
        IsDeleted = reader.GetOrdinal("order_item_deleted");
    }
}