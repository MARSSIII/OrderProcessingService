using Domain.Orders;
using Domain.Orders.Filters;

namespace Application.Abstractions.Repositories;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<Order>> InsertOrderAsync(IReadOnlyCollection<Order> orders, CancellationToken cancellationToken);

    Task UpdateOrderStateAsync(IReadOnlyCollection<Order> orders, CancellationToken cancellationToken);

    IAsyncEnumerable<Order> GetOrdersAsync(OrderFilter filter, CancellationToken cancellationToken);
}
