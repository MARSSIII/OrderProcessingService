using Domain.Orders;
using Domain.Orders.Filters;

namespace Application.Abstractions.Repositories;

public interface IOrderHistoryRepository
{
    Task<IReadOnlyCollection<OrderHistory>> InsertOrderHistoryAsync(IReadOnlyCollection<OrderHistory> orderHistories, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderHistory> GetOrdersAsync(OrderHistoryFilter filter, CancellationToken cancellationToken);
}
