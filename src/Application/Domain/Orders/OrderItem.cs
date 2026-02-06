namespace Domain.Orders;

public record OrderItem(
    long Id,
    long OrderId,
    long ProductId,
    int Quantity,
    bool IsDelete)
{
    public static OrderItem Create(long orderId, long productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException($"Quantity must be greater than zero. ProductId: {productId}", nameof(quantity));

        return new OrderItem(
            Id: 0,
            OrderId: orderId,
            ProductId: productId,
            Quantity: quantity,
            IsDelete: false);
    }
}
