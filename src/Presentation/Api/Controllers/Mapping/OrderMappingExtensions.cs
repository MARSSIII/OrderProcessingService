using Google.Protobuf.WellKnownTypes;

using Application.Contracts.Orders.DTOs;
using Application.Contracts.Orders.DTOs.Enums;
using Application.Contracts.Orders.DTOs.Payloads;
using Presentation.Protos.Orders.V1;

namespace Presentation.Api.Controllers.Mapping;

public static class OrderMappingExtensions
{
    public static OrderItemsCreateDto ToDto(this OrderItemCreate item)
    {
        return new OrderItemsCreateDto(item.ProductId, item.Quantity);
    }

    public static OrderItemResponse ToGrpcResponse(this OrderItemDetails item)
    {
        return new OrderItemResponse
        {
            OrderId = item.OrderId,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
        };
    }

    public static OrderHistoryResponse ToGrpcResponse(this OrderHistoryDetails historyEvent)
    {
        GrpcOrderHistoryItem grpcItemType = historyEvent.HistoryItem switch
        {
            OrderHistoryItemDetails.Created => GrpcOrderHistoryItem.Created,
            OrderHistoryItemDetails.ItemAdded => GrpcOrderHistoryItem.Added,
            OrderHistoryItemDetails.ItemRemoved => GrpcOrderHistoryItem.Removed,
            OrderHistoryItemDetails.StateChanged => GrpcOrderHistoryItem.StateChanged,
            _ => GrpcOrderHistoryItem.Unspecified,
        };

        var response = new OrderHistoryResponse
        {
            CreatedAt = Timestamp.FromDateTime(historyEvent.CreatedAt.ToUniversalTime()),
            HistoryItem = grpcItemType,
            Payload = new HistoryPayload(),
        };

        switch (historyEvent.Payload)
        {
            case OrderCreatedByPayloadDetails c:
                response.Payload.OrderCreated = new OrderCreatedPayload
                {
                    OrderCreatedBy = c.OrderCreatedBy,
                };
                break;

            case StatusChangedPayloadDetails s:
                response.Payload.StatusChanged = new StatusChangedPayload
                {
                    OldState = (GrpcOrderState)s.OldState,
                    NewState = (GrpcOrderState)s.NewState,
                };
                break;

            case ItemAddedPayloadDetails a:
                response.Payload.ItemAdded = new ItemAddedPayload
                {
                    ProductId = a.ProductId,
                    Quantity = a.Quantity,
                };
                break;

            case ItemDeletedPayloadDetails d:
                response.Payload.ItemDeleted = new ItemDeletedPayload
                {
                    ProductId = d.ProductId,
                };
                break;
        }

        return response;
    }
}
