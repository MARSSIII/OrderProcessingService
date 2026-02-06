using Domain.Orders.Enums;

namespace Domain.Orders.Filters;

public sealed record OrderHistoryFilter(
    long[] IdsOrders,
    OrderHistoryItem? HistoryItem,
    long Cursor,
    int PageSize)
{
    public static OrderHistoryFilter ByOrder(long orderId, long cursor, int pageSize)
    {
        return new OrderHistoryFilter(
            IdsOrders: new[] { orderId },
            HistoryItem: null,
            Cursor: cursor,
            PageSize: pageSize);
    }
}
