using System.Transactions;
using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Events;
using Application.Abstractions.Repositories;
using Domain.Orders;
using Domain.Orders.Filters;

namespace Application.Handlers;

public class OrderProcessingEventHandler : IOrderProcessingEventHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderHistoryRepository _orderHistoryRepository;

    public OrderProcessingEventHandler(
        IOrderRepository orderRepository,
        IOrderHistoryRepository orderHistoryRepository)
    {
        _orderRepository = orderRepository;
        _orderHistoryRepository = orderHistoryRepository;
    }

    public async Task HandleApprovalReceivedAsync(
        OrderApprovalEvent approvalEvent,
        CancellationToken cancellationToken)
    {
        if (!approvalEvent.IsApproved)
        {
            await TransitionOrderStateAsync(
                approvalEvent.OrderId,
                order => order.CancelInternal(),
                cancellationToken);
        }

        await TransitionOrderStateAsync(
            approvalEvent.OrderId,
            order => order.TransferToWork(),
            cancellationToken);
    }

    public async Task HandlePackingStartedAsync(
        OrderPackingStartedEvent packingEvent,
        CancellationToken cancellationToken)
    {
        await AddProcessingEventToHistoryAsync(packingEvent.OrderId, cancellationToken);
    }

    public async Task HandlePackingFinishedAsync(
        OrderPackingFinishedEvent packingEvent,
        CancellationToken cancellationToken)
    {
        if (!packingEvent.IsSuccessful)
        {
            await TransitionOrderStateAsync(
                packingEvent.OrderId,
                order => order.CancelInternal(),
                cancellationToken);
        }
    }

    public async Task HandleDeliveryStartedAsync(
        OrderDeliveryStartedEvent deliveryEvent,
        CancellationToken cancellationToken)
    {
        await AddProcessingEventToHistoryAsync(deliveryEvent.OrderId, cancellationToken);
    }

    public async Task HandleDeliveryFinishedAsync(
        OrderDeliveryFinishedEvent deliveryEvent,
        CancellationToken cancellationToken)
    {
        if (deliveryEvent.IsSuccessful)
        {
            await TransitionOrderStateAsync(
                deliveryEvent.OrderId,
                order => order.CompleteInternal(),
                cancellationToken);
        }
        else
        {
            await TransitionOrderStateAsync(
                deliveryEvent.OrderId,
                order => order.CancelInternal(),
                cancellationToken);
        }
    }

    private async Task TransitionOrderStateAsync(
        long orderId,
        Func<Order, Order> transitionAction,
        CancellationToken cancellationToken)
    {
        var scopeOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
        };

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            scopeOptions,
            TransactionScopeAsyncFlowOption.Enabled);

        Order? existingOrder = await GetOrderByIdAsync(orderId, cancellationToken);

        if (existingOrder is null)
        {
            return;
        }

        Order updatedOrder = transitionAction(existingOrder);

        if (updatedOrder.State == existingOrder.State)
        {
            scope.Complete();
            return;
        }

        var historyEntry = OrderHistory.StatusChanged(
            orderId: existingOrder.Id,
            oldState: existingOrder.State,
            newState: updatedOrder.State,
            createdAt: DateTime.UtcNow);

        await _orderRepository.UpdateOrderStateAsync(
            new[] { updatedOrder },
            cancellationToken);

        await _orderHistoryRepository.InsertOrderHistoryAsync(
            new[] { historyEntry },
            cancellationToken);

        scope.Complete();
    }

    private async Task<Order?> GetOrderByIdAsync(
        long orderId,
        CancellationToken cancellationToken)
    {
        var filter = OrderFilter.ByIds(new[] { orderId });

        await foreach (Order order in _orderRepository.GetOrdersAsync(filter, cancellationToken))
        {
            return order;
        }

        return null;
    }

    private async Task AddProcessingEventToHistoryAsync(
        long orderId,
        CancellationToken cancellationToken)
    {
        Order? order = await GetOrderByIdAsync(orderId, cancellationToken);

        if (order is null)
        {
            return;
        }

        var historyEntry = OrderHistory.StatusChanged(
            orderId: order.Id,
            oldState: order.State,
            newState: order.State,
            createdAt: DateTime.UtcNow);

        await _orderHistoryRepository.InsertOrderHistoryAsync(
            new[] { historyEntry },
            cancellationToken);
    }
}