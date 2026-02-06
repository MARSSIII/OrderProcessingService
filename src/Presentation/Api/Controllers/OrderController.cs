using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Application.Contracts.Orders;
using Application.Contracts.Orders.DTOs;

using Presentation.Api.Controllers.Mapping;
using Presentation.Protos.Orders.V1;

namespace Presentation.Api.Controllers;

public sealed class OrderController : OrderGrpcService.OrderGrpcServiceBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public override async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, ServerCallContext context)
    {
        OrderDetails createdOrder = await _orderService.CreateOrderAsync(request.CreatedBy, context.CancellationToken);

        return new OrderResponse
        {
            Id = createdOrder.Id,
        };
    }

    public override async Task<OrderItemListResponse> AddItemsToOrderAsync(AddItemsToOrderRequest request, ServerCallContext context)
    {
        var itemsToAdd = request.Items.Select(x => x.ToDto()).ToList();

        IReadOnlyCollection<OrderItemDetails> updatedItems = await _orderService.AddItemsToOrderAsync(request.OrderId, itemsToAdd, context.CancellationToken);

        var response = new OrderItemListResponse();
        response.OrderItems.AddRange(updatedItems.Select(x => x.ToGrpcResponse()));

        return response;
    }

    public override async Task<Empty> DeleteItemInOrderAsync(DeleteItemsInOrderRequest request, ServerCallContext context)
    {
        await _orderService.DeleteItemInOrderAsync(request.OrderId, request.ProductId, context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> TransferToWorkAsync(OrderIdRequest request, ServerCallContext context)
    {
        await _orderService.TransferToWorkAsync(request.OrderId, context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> CompleteOrderAsync(OrderIdRequest request, ServerCallContext context)
    {
        await _orderService.CompleteOrdersAsync(request.OrderId, context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> CancelOrderAsync(OrderIdRequest request, ServerCallContext context)
    {
        await _orderService.CancelledOrdersAsync(request.OrderId, context.CancellationToken);

        return new Empty();
    }

    public override async Task<OrderHistoryListResponse> GetOrderHistoryAsync(GetOrderHistoryRequest request, ServerCallContext context)
    {
        IReadOnlyCollection<OrderHistoryDetails> historyEvents = await _orderService.GetOrderHistoryAsync(
            request.OrderId,
            request.Cursor,
            request.PageSize,
            context.CancellationToken);

        var response = new OrderHistoryListResponse();

        response.History.AddRange(historyEvents.Select(h => h.ToGrpcResponse()));

        return response;
    }
}
