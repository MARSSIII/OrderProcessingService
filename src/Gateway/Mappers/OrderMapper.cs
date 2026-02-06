using Gateway.Models.Enums;
using Gateway.Models.Payloads;

using GrpcProtos = Presentation.Protos.Orders.V1;
using HttpRequests = Gateway.Models.Requests;
using HttpResponses = Gateway.Models.Responses;

namespace Gateway.Mappers;

public class OrderMapper
{
    public GrpcProtos.CreateOrderRequest ToGrpcCreateOrderRequest(HttpRequests.CreateOrderRequest request)
    {
        return new GrpcProtos.CreateOrderRequest
        {
            CreatedBy = request.CreatedBy,
        };
    }

    public GrpcProtos.AddItemsToOrderRequest ToGrpcAddItemsRequest(long orderId, HttpRequests.AddItemsToOrderRequest request)
    {
        var grpcRequest = new GrpcProtos.AddItemsToOrderRequest
        {
            OrderId = orderId,
        };

        foreach (HttpRequests.OrderItemCreateRequest item in request.Items)
        {
            grpcRequest.Items.Add(new GrpcProtos.OrderItemCreate
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
            });
        }

        return grpcRequest;
    }

    public GrpcProtos.DeleteItemsInOrderRequest ToGrpcDeleteItemsRequest(long orderId, long productId)
    {
        return new GrpcProtos.DeleteItemsInOrderRequest
        {
            OrderId = orderId,
            ProductId = productId,
        };
    }

    public GrpcProtos.OrderIdRequest ToGrpcOrderIdRequest(long orderId)
    {
        return new GrpcProtos.OrderIdRequest
        {
            OrderId = orderId,
        };
    }

    public GrpcProtos.GetOrderHistoryRequest ToGrpcGetOrderHistoryRequest(long orderId, long cursor, int pageSize)
    {
        return new GrpcProtos.GetOrderHistoryRequest
        {
            OrderId = orderId,
            Cursor = cursor,
            PageSize = pageSize,
        };
    }

    public HttpResponses.OrderResponse ToHttpOrderResponse(GrpcProtos.OrderResponse grpcResponse)
    {
        return new HttpResponses.OrderResponse(Id: grpcResponse.Id);
    }

    public HttpResponses.OrderItemListResponse ToHttpOrderItemListResponse(GrpcProtos.OrderItemListResponse grpcResponse)
    {
        var items = grpcResponse.OrderItems
            .Select(item => new HttpResponses.OrderItemResponse(
                OrderId: item.OrderId,
                ProductId: item.ProductId,
                Quantity: item.Quantity))
            .ToList();

        return new HttpResponses.OrderItemListResponse(Items: items);
    }

    public HttpResponses.OrderHistoryListResponse ToHttpOrderHistoryListResponse(GrpcProtos.OrderHistoryListResponse grpcResponse)
    {
        var history = grpcResponse.History
            .Select(MapOrderHistoryResponse)
            .ToList();

        return new HttpResponses.OrderHistoryListResponse(History: history);
    }

    private static HttpResponses.OrderHistoryResponse MapOrderHistoryResponse(GrpcProtos.OrderHistoryResponse grpcHistory)
    {
        return new HttpResponses.OrderHistoryResponse(
            CreatedAt: grpcHistory.CreatedAt.ToDateTime(),
            HistoryItemType: MapHistoryItemType(grpcHistory.HistoryItem),
            Payload: MapPayload(grpcHistory.Payload));
    }

    private static OrderHistoryItemType MapHistoryItemType(GrpcProtos.GrpcOrderHistoryItem grpcType)
    {
        return grpcType switch
        {
            GrpcProtos.GrpcOrderHistoryItem.Created => OrderHistoryItemType.Created,
            GrpcProtos.GrpcOrderHistoryItem.Added => OrderHistoryItemType.Added,
            GrpcProtos.GrpcOrderHistoryItem.Removed => OrderHistoryItemType.Removed,
            GrpcProtos.GrpcOrderHistoryItem.StateChanged => OrderHistoryItemType.StateChanged,
            GrpcProtos.GrpcOrderHistoryItem.Unspecified => throw new NotImplementedException(),
            _ => OrderHistoryItemType.Unspecified,
        };
    }

    private static OrderState MapOrderState(GrpcProtos.GrpcOrderState grpcState)
    {
        return grpcState switch
        {
            GrpcProtos.GrpcOrderState.Created => OrderState.Created,
            GrpcProtos.GrpcOrderState.Processing => OrderState.Processing,
            GrpcProtos.GrpcOrderState.Completed => OrderState.Completed,
            GrpcProtos.GrpcOrderState.Cancelled => OrderState.Cancelled,
            GrpcProtos.GrpcOrderState.Unspecified => throw new NotImplementedException(),
            _ => OrderState.Unspecified,
        };
    }

    private static HistoryPayloadBase? MapPayload(GrpcProtos.HistoryPayload? grpcPayload)
    {
        if (grpcPayload is null)
            return null;

        return grpcPayload.KindCase switch
        {
            GrpcProtos.HistoryPayload.KindOneofCase.OrderCreated =>
                new OrderCreatedPayload(grpcPayload.OrderCreated.OrderCreatedBy),

            GrpcProtos.HistoryPayload.KindOneofCase.StatusChanged =>
                new StatusChangedPayload(
                    MapOrderState(grpcPayload.StatusChanged.OldState),
                    MapOrderState(grpcPayload.StatusChanged.NewState)),
            GrpcProtos.HistoryPayload.KindOneofCase.ItemAdded =>
                new ItemAddedPayload(
                    grpcPayload.ItemAdded.ProductId,
                    grpcPayload.ItemAdded.Quantity),
            GrpcProtos.HistoryPayload.KindOneofCase.ItemDeleted =>
                new ItemDeletedPayload(grpcPayload.ItemDeleted.ProductId),
            GrpcProtos.HistoryPayload.KindOneofCase.None =>
                throw new NotImplementedException(),
            _ => null,
        };
    }
}