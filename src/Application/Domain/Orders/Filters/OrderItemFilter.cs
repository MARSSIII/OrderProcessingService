namespace Domain.Orders.Filters;

public sealed record OrderItemFilter(
    long[]? IdsOrders,
    long[]? IdsProducts,
    bool? IsDelete,
    long Cursor,
    int PageSize)
{
    public static OrderItemFilter Create(long orderId, long productId)
    {
        return new OrderItemFilter(
            IdsOrders: new[] { orderId },
            IdsProducts: new[] { productId },
            IsDelete: false,
            Cursor: 0,
            PageSize: int.MaxValue);
    }
}
