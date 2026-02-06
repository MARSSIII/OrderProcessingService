using Npgsql;
using Domain.Orders;
using Infrastructure.Postgres.Mapping.Ordinals;

namespace Infrastructure.Postgres.Mapping.Mappers;

public static class OrderItemMapper
{
    public static OrderItem Map(NpgsqlDataReader reader, OrderItemOrdinals ordinals)
    {
        return new OrderItem(
            Id: reader.GetInt64(ordinals.Id),
            OrderId: reader.GetInt64(ordinals.OrderId),
            ProductId: reader.GetInt64(ordinals.ProductId),
            Quantity: reader.GetInt32(ordinals.Quantity),
            IsDelete: reader.GetBoolean(ordinals.IsDeleted));
    }
}