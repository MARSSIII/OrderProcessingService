using Domain.Orders.Enums;
using Domain.Orders.Payloads;

namespace Domain.Orders;

public record OrderHistory(
    long? Id,
    long OrderId,
    DateTime CreatedAt,
    OrderHistoryItem HistoryItem,
    PayloadBase Payload)
{
    public static OrderHistory Created(long orderId, string createdBy, DateTime createdAt)
    {
        return new OrderHistory(
            Id: null,
            OrderId: orderId,
            CreatedAt: createdAt,
            HistoryItem: OrderHistoryItem.Created,
            Payload: new OrderCreatedByPayload(createdBy));
    }

    public static OrderHistory StatusChanged(long orderId, OrderState oldState, OrderState newState, DateTime createdAt)
    {
        return new OrderHistory(
            Id: null,
            OrderId: orderId,
            CreatedAt: createdAt,
            HistoryItem: OrderHistoryItem.StateChanged,
            Payload: new StatusChangedPayload(oldState, newState));
    }

    public static OrderHistory ItemAdded(long orderId, long productId, int quantity, DateTime createdAt)
    {
        return new OrderHistory(
            Id: null,
            OrderId: orderId,
            CreatedAt: createdAt,
            HistoryItem: OrderHistoryItem.ItemAdded,
            Payload: new ItemAddedPayload(productId, quantity));
    }

    public static OrderHistory ItemDeleted(long orderId, long productId, DateTime createdAt)
    {
        return new OrderHistory(
            Id: null,
            OrderId: orderId,
            CreatedAt: createdAt,
            HistoryItem: OrderHistoryItem.ItemRemoved,
            Payload: new ItemDeletedPayload(productId));
    }
}
