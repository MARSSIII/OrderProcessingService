using Npgsql;
using System.Text.Json;
using Domain.Orders;
using Domain.Orders.Enums;
using Domain.Orders.Payloads;
using Infrastructure.Postgres.Mapping.Ordinals;

namespace Infrastructure.Postgres.Mapping.Mappers;

public static class OrderHistoryMapper
{
    public static OrderHistory Map(NpgsqlDataReader reader, OrderHistoryOrdinals ordinals)
    {
        long id = reader.GetInt64(ordinals.Id);
        long orderId = reader.GetInt64(ordinals.OrderId);
        DateTime createdAt = reader.GetFieldValue<DateTime>(ordinals.CreatedAt);
        OrderHistoryItem historyItem = reader.GetFieldValue<OrderHistoryItem>(ordinals.HistoryItem);
        string payloadJson = reader.GetString(ordinals.Payload);

        PayloadBase payload = DeserializePayload(historyItem, payloadJson, id);

        return new OrderHistory(id, orderId, createdAt, historyItem, payload);
    }

    private static PayloadBase DeserializePayload(OrderHistoryItem historyItem, string payloadJson, long itemId)
    {
        PayloadBase? payload = historyItem switch
        {
            OrderHistoryItem.Created => JsonSerializer.Deserialize<OrderCreatedByPayload>(payloadJson),
            OrderHistoryItem.StateChanged => JsonSerializer.Deserialize<StatusChangedPayload>(payloadJson),
            OrderHistoryItem.ItemAdded => JsonSerializer.Deserialize<ItemAddedPayload>(payloadJson),
            OrderHistoryItem.ItemRemoved => JsonSerializer.Deserialize<ItemDeletedPayload>(payloadJson),
            _ => throw new InvalidOperationException($"Unknown history item type: {historyItem}"),
        };

        if (payload is null)
        {
            throw new InvalidOperationException($"Failed to deserialize payload for history item {itemId}.");
        }

        return payload;
    }
}