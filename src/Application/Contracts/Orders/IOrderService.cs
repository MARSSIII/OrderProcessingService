using Application.Contracts.Orders.DTOs;

namespace Application.Contracts.Orders;

public interface IOrderService
{
    Task<OrderDetails> CreateOrderAsync(
            string createdBy,
            CancellationToken cancellationToken);

    Task<IReadOnlyCollection<OrderItemDetails>> AddItemsToOrderAsync(
            long orderId,
            IReadOnlyCollection<OrderItemsCreateDto> items,
            CancellationToken cancellationToken);

    Task DeleteItemInOrderAsync(
            long orderId,
            long productId,
            CancellationToken cancellationToken);

    Task TransferToWorkAsync(
            long orderId,
            CancellationToken cancellationToken);

    Task CancelledOrdersAsync(
            long orderId,
            CancellationToken cancellationToken);

    Task CompleteOrdersAsync(
            long orderId,
            CancellationToken cancellationToken);

    Task<IReadOnlyCollection<OrderHistoryDetails>> GetOrderHistoryAsync(
            long orderId,
            long cursor,
            int pageSize,
            CancellationToken cancellationToken);
}
