using Gateway.Mappers;

using GrpcProtos = Presentation.Protos.Orders.V1;
using HttpRequests = Gateway.Models.Requests;
using HttpResponses = Gateway.Models.Responses;

namespace Gateway.Clients.GrpcOrders;

public class OrderGrpcClient : IOrderGrpcClient
{
    private readonly GrpcProtos.OrderGrpcService.OrderGrpcServiceClient _client;

    private readonly OrderMapper _mapper;

    public OrderGrpcClient(
        GrpcProtos.OrderGrpcService.OrderGrpcServiceClient client,
        OrderMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public async Task<HttpResponses.OrderResponse> CreateOrderAsync(
        HttpRequests.CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        GrpcProtos.CreateOrderRequest grpcRequest = _mapper.ToGrpcCreateOrderRequest(request);

        GrpcProtos.OrderResponse response = await _client.CreateOrderAsyncAsync(grpcRequest, cancellationToken: cancellationToken);

        return _mapper.ToHttpOrderResponse(response);
    }

    public async Task<HttpResponses.OrderItemListResponse> AddItemsToOrderAsync(
        long orderId,
        HttpRequests.AddItemsToOrderRequest request,
        CancellationToken cancellationToken)
    {
        GrpcProtos.AddItemsToOrderRequest grpcRequest = _mapper.ToGrpcAddItemsRequest(orderId, request);

        GrpcProtos.OrderItemListResponse response = await _client.AddItemsToOrderAsyncAsync(grpcRequest, cancellationToken: cancellationToken);

        return _mapper.ToHttpOrderItemListResponse(response);
    }

    public async Task DeleteItemInOrderAsync(long orderId, long productId, CancellationToken cancellationToken)
    {
        GrpcProtos.DeleteItemsInOrderRequest grpcRequest = _mapper.ToGrpcDeleteItemsRequest(orderId, productId);

        await _client.DeleteItemInOrderAsyncAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task TransferToWorkAsync(long orderId, CancellationToken cancellationToken)
    {
        GrpcProtos.OrderIdRequest grpcRequest = _mapper.ToGrpcOrderIdRequest(orderId);

        await _client.TransferToWorkAsyncAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task CancelOrderAsync(long orderId, CancellationToken cancellationToken)
    {
        GrpcProtos.OrderIdRequest grpcRequest = _mapper.ToGrpcOrderIdRequest(orderId);

        await _client.CancelOrderAsyncAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task<HttpResponses.OrderHistoryListResponse> GetOrderHistoryAsync(
        long orderId,
        long cursor,
        int pageSize,
        CancellationToken cancellationToken)
    {
        GrpcProtos.GetOrderHistoryRequest grpcRequest = _mapper.ToGrpcGetOrderHistoryRequest(orderId, cursor, pageSize);

        GrpcProtos.OrderHistoryListResponse response = await _client.GetOrderHistoryAsyncAsync(grpcRequest, cancellationToken: cancellationToken);

        return _mapper.ToHttpOrderHistoryListResponse(response);
    }
}