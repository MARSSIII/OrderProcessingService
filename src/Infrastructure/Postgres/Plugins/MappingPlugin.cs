using Npgsql;
using Domain.Orders.Enums;

namespace Infrastructure.Postgres.Plugins;

public class MappingPlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSourceBuilder)
    {
        dataSourceBuilder
            .MapEnum<OrderHistoryItem>("order_history_item_kind")
            .MapEnum<OrderState>("order_state");
    }
}
