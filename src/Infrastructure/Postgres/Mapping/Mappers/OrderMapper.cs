using Npgsql;
using Domain.Orders;
using Domain.Orders.Enums;
using Infrastructure.Postgres.Mapping.Ordinals;

namespace Infrastructure.Postgres.Mapping.Mappers;

public static class OrderMapper
{
    public static Order Map(NpgsqlDataReader reader, OrderOrdinals ordinals)
    {
        return new Order(
            Id: reader.GetInt64(ordinals.Id),
            State: reader.GetFieldValue<OrderState>(ordinals.State),
            CreatedAt: reader.GetFieldValue<DateTime>(ordinals.CreatedAt),
            CreatedBy: reader.GetString(ordinals.CreatedBy));
    }
}