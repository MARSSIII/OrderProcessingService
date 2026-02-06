using System.Transactions;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;

using Application.Contracts.Orders;
using Application.Contracts.Orders.DTOs;
using Application.Mapping;
using Domain.Orders;
using Domain.Orders.Filters;

namespace Application.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    private readonly IOrderItemRepository _orderItemRepository;

    private readonly IOrderHistoryRepository _orderHistoryRepository;

    private readonly IOrderEventPublisher _eventPublisher;

    public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IOrderHistoryRepository orderHistoryRepository,
            IOrderEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _orderHistoryRepository = orderHistoryRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<OrderDetails> CreateOrderAsync(
        string createdBy,
        CancellationToken cancellationToken)
    {
        DateTime now = DateTime.UtcNow;
        var orderToInsert = Order.Create(
                createdBy: createdBy,
                dateTime: now);

        var scopeOption = new TransactionOptions()
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.FromSeconds(30),
        };

        using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                scopeOption,
                TransactionScopeAsyncFlowOption.Enabled);

        IReadOnlyCollection<Order> createdOrders = await _orderRepository.InsertOrderAsync(
                new[] { orderToInsert },
                cancellationToken);

        Order createdOrder = createdOrders.Single();

        var historyEntry = OrderHistory.Created(
                orderId: createdOrder.Id,
                createdBy: createdOrder.CreatedBy,
                createdAt: now);

        await _orderHistoryRepository.InsertOrderHistoryAsync(
                new[] { historyEntry },
                cancellationToken);

        scope.Complete();

        await _eventPublisher.PublishOrderCreatedAsync(createdOrder.Id, cancellationToken);

        return createdOrder.ToDetails();
    }

    public async Task<IReadOnlyCollection<OrderItemDetails>> AddItemsToOrderAsync(
            long orderId,
            IReadOnlyCollection<OrderItemsCreateDto> items,
            CancellationToken cancellationToken)
    {
        var itemsToInsert = new List<OrderItem>(items.Count);

        foreach (OrderItemsCreateDto dto in items)
        {
            itemsToInsert.Add(OrderItem.Create(
                        orderId: orderId,
                        productId: dto.ProductId,
                        quantity: dto.Quantity));
        }

        var scopeOption = new TransactionOptions()
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
        };

        using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                scopeOption,
                TransactionScopeAsyncFlowOption.Enabled);

        IReadOnlyCollection<OrderItem> insertedItems = await _orderItemRepository.InsertOrderItemAsync(itemsToInsert, cancellationToken);

        var historyEntries = new List<OrderHistory>(insertedItems.Count);
        DateTime now = DateTime.UtcNow;

        foreach (OrderItem item in insertedItems)
        {
            historyEntries.Add(OrderHistory.ItemAdded(
                orderId: item.OrderId,
                productId: item.ProductId,
                quantity: item.Quantity,
                createdAt: now));
        }

        await _orderHistoryRepository.InsertOrderHistoryAsync(historyEntries, cancellationToken);

        scope.Complete();

        return insertedItems.ToDetails();
    }

    public async Task DeleteItemInOrderAsync(
        long orderId,
        long productId,
        CancellationToken cancellationToken)
    {
        var filter = OrderItemFilter.Create(orderId, productId);

        var itemsToDelete = new List<OrderItem>();

        var scopeOption = new TransactionOptions()
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
        };

        using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                scopeOption,
                TransactionScopeAsyncFlowOption.Enabled);

        await foreach (OrderItem item in _orderItemRepository.GetItemsOrderAsync(filter, cancellationToken))
        {
            itemsToDelete.Add(item);
        }

        if (itemsToDelete.Count == 0)
            return;

        await _orderItemRepository.DeleteOrderItemAsync(itemsToDelete, cancellationToken);

        var historyEntries = new List<OrderHistory>(itemsToDelete.Count);
        DateTime now = DateTime.UtcNow;

        foreach (OrderItem item in itemsToDelete)
        {
            historyEntries.Add(OrderHistory.ItemDeleted(
                orderId: item.OrderId,
                productId: item.ProductId,
                createdAt: now));
        }

        await _orderHistoryRepository.InsertOrderHistoryAsync(historyEntries, cancellationToken);

        scope.Complete();
    }

    public async Task TransferToWorkAsync(
            long orderId,
            CancellationToken cancellationToken)
    {
        await ChangeStatusAsync(
            orderId,
            order => order.TransferToWork(),
            cancellationToken);

        await _eventPublisher.PublishOrderProcessingStartedAsync(orderId, cancellationToken);
    }

    public async Task CancelledOrdersAsync(
        long orderId,
        CancellationToken cancellationToken)
    {
        await ChangeStatusAsync(
            orderId,
            order => order.Cancel(),
            cancellationToken);
    }

    public async Task CompleteOrdersAsync(
        long orderId,
        CancellationToken cancellationToken)
    {
        await ChangeStatusAsync(
            orderId,
            order => order.Complete(),
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<OrderHistoryDetails>> GetOrderHistoryAsync(
        long orderId,
        long cursor,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (pageSize <= 0)
            throw new ArgumentException("PageSize must be greater than zero.", nameof(pageSize));

        var filter = OrderHistoryFilter.ByOrder(
                orderId: orderId,
                cursor: cursor,
                pageSize: pageSize);

        var historyList = new List<OrderHistory>(pageSize);

        await foreach (OrderHistory item in _orderHistoryRepository.GetOrdersAsync(filter, cancellationToken))
        {
            historyList.Add(item);
        }

        return historyList.ToDetails();
    }

    private async Task ChangeStatusAsync(
            long orderId,
            Func<Order, Order> transitionAction,
            CancellationToken cancellationToken)
    {
        var filter = OrderFilter.ByIds(ids: new[] { orderId });

        var scopeOption = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
        };

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            scopeOption,
            TransactionScopeAsyncFlowOption.Enabled);

        Order? existingOrder = null;

        await foreach (Order order in _orderRepository.GetOrdersAsync(filter, cancellationToken))
        {
            existingOrder = order;
            break;
        }

        if (existingOrder == null)
            return;

        Order updatedOrder = transitionAction(existingOrder);

        if (updatedOrder.State == existingOrder.State)
        {
            scope.Complete();

            return;
        }

        DateTime now = DateTime.UtcNow;

        var historyEntry = OrderHistory.StatusChanged(
            orderId: existingOrder.Id,
            oldState: existingOrder.State,
            newState: updatedOrder.State,
            createdAt: now);

        await _orderRepository.UpdateOrderStateAsync(new[] { updatedOrder }, cancellationToken);
        await _orderHistoryRepository.InsertOrderHistoryAsync(new[] { historyEntry }, cancellationToken);

        scope.Complete();
    }
}
