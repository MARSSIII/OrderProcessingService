using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using Application.Abstractions.Repositories;
using Domain.Orders;
using Domain.Orders.Enums;
using Domain.Orders.Filters;
using Infrastructure.Postgres.Mapping.Mappers;
using Infrastructure.Postgres.Mapping.Ordinals;

namespace Infrastructure.Postgres.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IReadOnlyCollection<Order>> InsertOrderAsync(IReadOnlyCollection<Order> orders, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO orders(order_state, order_created_at, order_created_by)
        SELECT state, created_at, created_by FROM unnest(:states, :created_ats, :created_bys) as source(state, created_at, created_by)
        RETURNING order_id;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("states", orders.Select(x => x.State).ToArray()),
                new NpgsqlParameter("created_ats", orders.Select(x => x.CreatedAt).ToArray()),
                new NpgsqlParameter("created_bys", orders.Select(x => x.CreatedBy).ToArray()),
            },
        };

        var orderList = orders.ToList();
        var ids = new List<long>(orderList.Count);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            ids.Add(reader.GetInt64(reader.GetOrdinal("order_id")));
        }

        if (ids.Count != orderList.Count)
            throw new InvalidOperationException("Order insertion operation failed: ids count mismatch.");

        var result = new List<Order>(orderList.Count);
        for (int i = 0; i < orderList.Count; i++)
        {
            result.Add(orderList[i] with { Id = ids[i] });
        }

        return result;
    }

    public async Task UpdateOrderStateAsync(IReadOnlyCollection<Order> orders, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE orders
            SET order_state = source.state
            FROM(select * from unnest(:ids, :states)) as source(id, state)
             WHERE order_id = source.id;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids", orders.Select(x => x.Id).ToArray()),
                new NpgsqlParameter("states", orders.Select(x => x.State).ToArray()),
            },
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async IAsyncEnumerable<Order> GetOrdersAsync(OrderFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT order_id, order_state, order_created_at, order_created_by
        FROM orders
        WHERE
             (order_id > :cursor)
            AND (cardinality(:ids) = 0 OR order_id = ANY(:ids))
            AND (:state IS NULL OR order_state != :state)
            AND (:created_by IS NULL OR order_created_by ILIKE :created_by)
        ORDER BY order_id
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
                new NpgsqlParameter<OrderState?>("state", filter.State),
                new NpgsqlParameter("created_by", NpgsqlDbType.Text)
                {
                    Value = filter.CreatedBy is null
                        ? DBNull.Value
                        : filter.CreatedBy,
                },
                new NpgsqlParameter("pageSize", filter.PageSize),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        var ordinals = new OrderOrdinals(reader);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return OrderMapper.Map(reader, ordinals);
        }
    }
}
