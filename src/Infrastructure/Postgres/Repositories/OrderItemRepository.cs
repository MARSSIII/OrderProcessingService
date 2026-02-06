using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using Application.Abstractions.Repositories;
using Domain.Orders;
using Domain.Orders.Filters;
using Infrastructure.Postgres.Mapping.Mappers;
using Infrastructure.Postgres.Mapping.Ordinals;

namespace Infrastructure.Postgres.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderItemRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IReadOnlyCollection<OrderItem>> InsertOrderItemAsync(IReadOnlyCollection<OrderItem> orderItems, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO order_items(order_id, product_id, order_item_quantity, order_item_deleted)
        SELECT id_order, id_product, item_quantity, item_deleted
        FROM unnest(:ids_orders, :ids_products, :item_quantities, :item_deletes) as source(id_order, id_product, item_quantity, item_deleted)
        RETURNING order_item_id;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids_orders", orderItems.Select(x => x.OrderId).ToArray()),
                new NpgsqlParameter("ids_products", orderItems.Select(x => x.ProductId).ToArray()),
                new NpgsqlParameter("item_quantities", orderItems.Select(x => x.Quantity).ToArray()),
                new NpgsqlParameter("item_deletes", orderItems.Select(x => x.IsDelete).ToArray()),
            },
        };

        var itemsList = orderItems.ToList();
        var ids = new List<long>(itemsList.Count);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            ids.Add(reader.GetInt64(reader.GetOrdinal("order_item_id")));
        }

        if (ids.Count != itemsList.Count)
            throw new InvalidOperationException("Order items insertion operation failed: ids count mismatch.");

        var result = new List<OrderItem>(itemsList.Count);
        for (int i = 0; i < itemsList.Count; i++)
        {
            result.Add(itemsList[i] with { Id = ids[i] });
        }

        return result;
    }

    public async Task<IReadOnlyCollection<OrderItem>> DeleteOrderItemAsync(IReadOnlyCollection<OrderItem> orderItems, CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE order_items
        SET order_item_deleted = TRUE
        FROM unnest(:ids_orders, :ids_products) as source(ids_orders, ids_products)
        WHERE(order_id = source.ids_orders)
        AND(product_id = source.ids_products)
        RETURNING order_item_id, order_id, product_id, order_item_quantity, order_item_deleted;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("ids_orders", orderItems.Select(x => x.OrderId).ToArray()),
                new NpgsqlParameter("ids_products", orderItems.Select(x => x.ProductId).ToArray()),
            },
        };

        var result = new List<OrderItem>();

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        var ordinals = new OrderItemOrdinals(reader);
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(OrderItemMapper.Map(reader, ordinals));
        }

        if (result.Count == 0)
        {
            throw new InvalidOperationException("Items not found or already deleted");
        }

        return result;
    }

    public async IAsyncEnumerable<OrderItem> GetItemsOrderAsync(OrderItemFilter filter, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
        FROM order_items
        WHERE
             (order_item_id > :cursor)
            AND(cardinality(:ids_products) = 0 OR product_id = ANY(:ids_products))
            AND(cardinality(:ids_orders) = 0 OR order_id = ANY(:ids_orders))
            AND(:is_delete IS NULL OR order_item_deleted != :is_delete)
        ORDER BY order_item_id
        LIMIT :pageSize;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("cursor", filter.Cursor),
                new NpgsqlParameter("ids_products", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
                {
                    Value = filter.IdsProducts ?? [],
                },
                new NpgsqlParameter("ids_orders", NpgsqlDbType.Array | NpgsqlDbType.Bigint)
                {
                    Value = filter.IdsOrders ?? [],
                },
                new NpgsqlParameter("is_delete", NpgsqlDbType.Boolean)
                {
                    Value = filter.IsDelete.HasValue
                            ? filter.IsDelete.Value
                            : DBNull.Value,
                },
                new NpgsqlParameter("pageSize", filter.PageSize),
            },
        };

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        var ordinals = new OrderItemOrdinals(reader);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return OrderItemMapper.Map(reader, ordinals);
        }
    }
}
