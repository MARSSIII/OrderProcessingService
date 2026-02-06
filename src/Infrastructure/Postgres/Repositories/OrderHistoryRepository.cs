using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Application.Abstractions.Repositories;
using Domain.Orders;
using Domain.Orders.Enums;
using Domain.Orders.Filters;
using Infrastructure.Postgres.Mapping.Mappers;
using Infrastructure.Postgres.Mapping.Ordinals;

namespace Infrastructure.Postgres.Repositories;

public class OrderHistoryRepository : IOrderHistoryRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderHistoryRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IReadOnlyCollection<OrderHistory>> InsertOrderHistoryAsync(IReadOnlyCollection<OrderHistory> orderHistories, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO order_history(order_id, order_history_item_created_at, order_history_item_kind, order_history_item_payload)
        SELECT id_order, created_at, item_kind, item_payload FROM unnest(:ids_orders, :created_ats, :items_kind, :items_payload) as source(id_order, created_at, item_kind, item_payload)
        RETURNING order_history_item_id;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids_orders", orderHistories.Select(x => x.OrderId).ToArray()),
                new NpgsqlParameter("created_ats", orderHistories.Select(x => x.CreatedAt).ToArray()),
                new NpgsqlParameter("items_kind", orderHistories.Select(x => x.HistoryItem).ToArray()),
                new NpgsqlParameter("items_payload", NpgsqlDbType.Jsonb | NpgsqlDbType.Array)
                {
                    Value = orderHistories.Select(x => JsonSerializer.Serialize(x.Payload)).ToArray(),
                },
            },
        };

        var historiesList = orderHistories.ToList();
        var ids = new List<long>(historiesList.Count);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            ids.Add(reader.GetInt64(reader.GetOrdinal("order_history_item_id")));
        }

        if (ids.Count != historiesList.Count)
            throw new InvalidOperationException("Order history insertion operation failed: ids count mismatch.");

        var result = new List<OrderHistory>(historiesList.Count);
        for (int i = 0; i < historiesList.Count; i++)
        {
            result.Add(historiesList[i] with { Id = ids[i] });
        }

        return result;
    }

    public async IAsyncEnumerable<OrderHistory> GetOrdersAsync(OrderHistoryFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT order_history_item_id, order_id, order_history_item_created_at, order_history_item_kind, order_history_item_payload
        FROM order_history
        WHERE
             (order_history_item_id > :cursor)
            AND (cardinality(:ids_orders) = 0 OR order_id = ANY(:ids_orders))
            AND (:history_item IS NULL OR order_history_item_kind != :history_item)
        ORDER BY order_history_item_id
        LIMIT :pageSize;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", filter.Cursor),
                new NpgsqlParameter("ids_orders", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
                {
                    Value = filter.IdsOrders ?? [],
                },
                new NpgsqlParameter<OrderHistoryItem?>("history_item", filter.HistoryItem),
                new NpgsqlParameter("pageSize", filter.PageSize),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        var ordinals = new OrderHistoryOrdinals(reader);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return OrderHistoryMapper.Map(reader, ordinals);
        }
    }
}
