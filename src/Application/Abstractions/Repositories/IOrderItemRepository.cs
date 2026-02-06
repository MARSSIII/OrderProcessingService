using Domain.Orders;
using Domain.Orders.Filters;

namespace Application.Abstractions.Repositories;

public interface IOrderItemRepository
{
    Task<IReadOnlyCollection<OrderItem>> InsertOrderItemAsync(IReadOnlyCollection<OrderItem> orderItems, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<OrderItem>> DeleteOrderItemAsync(IReadOnlyCollection<OrderItem> orderItems, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderItem> GetItemsOrderAsync(OrderItemFilter filter, CancellationToken cancellationToken);
}
