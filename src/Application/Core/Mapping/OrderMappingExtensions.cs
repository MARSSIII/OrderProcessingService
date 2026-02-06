using Application.Contracts.Orders.DTOs;
using Application.Contracts.Orders.DTOs.Enums;
using Application.Contracts.Orders.DTOs.Payloads;
using Domain.Orders;
using Domain.Orders.Enums;
using Domain.Orders.Payloads;

namespace Application.Mapping;

public static class OrderMappingExtensions
{
    public static OrderDetails ToDetails(this Order order)
    {
        return new OrderDetails(Id: order.Id);
    }

    public static OrderItemDetails ToDetails(this OrderItem item)
    {
        return new OrderItemDetails(
            OrderId: item.OrderId,
            ProductId: item.ProductId,
            Quantity: item.Quantity);
    }

    public static IReadOnlyCollection<OrderItemDetails> ToDetails(
        this IReadOnlyCollection<OrderItem> items)
    {
        return items.Select(i => i.ToDetails()).ToList();
    }

    public static OrderHistoryDetails ToDetails(this OrderHistory history)
    {
        return new OrderHistoryDetails(
            CreatedAt: history.CreatedAt,
            HistoryItem: history.HistoryItem.ToDetails(),
            Payload: history.Payload.ToDetails());
    }

    public static IReadOnlyCollection<OrderHistoryDetails> ToDetails(
        this IReadOnlyCollection<OrderHistory> history)
    {
        return history.Select(h => h.ToDetails()).ToList();
    }

    public static OrderStateDetails ToDetails(this OrderState state)
    {
        return state switch
        {
            OrderState.Created => OrderStateDetails.Created,
            OrderState.Processing => OrderStateDetails.Processing,
            OrderState.Completed => OrderStateDetails.Completed,
            OrderState.Cancelled => OrderStateDetails.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Unknown order state"),
        };
    }

    public static OrderHistoryItemDetails ToDetails(this OrderHistoryItem item)
    {
        return item switch
        {
            OrderHistoryItem.Created => OrderHistoryItemDetails.Created,
            OrderHistoryItem.ItemAdded => OrderHistoryItemDetails.ItemAdded,
            OrderHistoryItem.ItemRemoved => OrderHistoryItemDetails.ItemRemoved,
            OrderHistoryItem.StateChanged => OrderHistoryItemDetails.StateChanged,
            _ => throw new ArgumentOutOfRangeException(nameof(item), item, "Unknown history item type"),
        };
    }

    public static PayloadBaseDetails ToDetails(this PayloadBase payload)
    {
        return payload switch
        {
            OrderCreatedByPayload p => new OrderCreatedByPayloadDetails(p.OrderCreatedBy),
            StatusChangedPayload p => new StatusChangedPayloadDetails(p.OldState.ToDetails(), p.NewState.ToDetails()),
            ItemAddedPayload p => new ItemAddedPayloadDetails(p.ProductId, p.Quantity),
            ItemDeletedPayload p => new ItemDeletedPayloadDetails(p.ProductId),
            _ => throw new ArgumentOutOfRangeException(nameof(payload), payload.GetType().Name, "Unknown payload type"),
        };
    }
}