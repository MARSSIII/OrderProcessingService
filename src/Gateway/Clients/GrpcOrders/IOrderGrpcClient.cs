using Gateway.Models.Requests;
using Gateway.Models.Responses;

namespace Gateway.Clients.GrpcOrders;

public interface IOrderGrpcClient
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken);

    Task<OrderItemListResponse> AddItemsToOrderAsync(long orderId, AddItemsToOrderRequest request, CancellationToken cancellationToken);

    Task DeleteItemInOrderAsync(long orderId, long productId, CancellationToken cancellationToken);

    Task TransferToWorkAsync(long orderId, CancellationToken cancellationToken);

    Task CancelOrderAsync(long orderId, CancellationToken cancellationToken);

    Task<OrderHistoryListResponse> GetOrderHistoryAsync(long orderId, long cursor, int pageSize, CancellationToken cancellationToken);
}