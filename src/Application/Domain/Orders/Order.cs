using Domain.Orders.Enums;

namespace Domain.Orders;

public record Order(
    long Id,
    OrderState State,
    DateTime CreatedAt,
    string CreatedBy)
{
    public static Order Create(string createdBy, DateTime dateTime)
    {
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy cannot be empty.");

        return new Order(
            Id: 0,
            State: OrderState.Created,
            CreatedAt: dateTime,
            CreatedBy: createdBy);
    }

    public Order TransferToWork()
    {
        if (State != OrderState.Created)
        {
            throw new InvalidOperationException(
                $"Order {Id} cannot be transferred to work. Current state: '{State}'. Only 'Created' orders can be processed.");
        }

        return this with { State = OrderState.Processing };
    }

    public Order Complete()
    {
        if (State != OrderState.Processing)
            throw new InvalidOperationException($"Order {Id} is in '{State}' state. Only 'Processing' orders can be completed.");

        return this with { State = OrderState.Completed };
    }

    public Order Cancel()
    {
        if (State == OrderState.Cancelled)
            return this;

        if (State != OrderState.Created)
            throw new InvalidOperationException($"Order {Id} is Completed and cannot be cancelled.");

        return this with { State = OrderState.Cancelled };
    }

    public Order CancelInternal()
    {
        return this with { State = OrderState.Cancelled };
    }

    public Order CompleteInternal()
    {
        return this with { State = OrderState.Completed };
    }
}
