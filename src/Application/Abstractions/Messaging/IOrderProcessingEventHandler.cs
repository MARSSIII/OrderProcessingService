using Application.Abstractions.Messaging.Events;

namespace Application.Abstractions.Messaging;

public interface IOrderProcessingEventHandler
{
    Task HandleApprovalReceivedAsync(OrderApprovalEvent approvalEvent, CancellationToken cancellationToken);

    Task HandlePackingStartedAsync(OrderPackingStartedEvent packingEvent, CancellationToken cancellationToken);

    Task HandlePackingFinishedAsync(OrderPackingFinishedEvent packingEvent, CancellationToken cancellationToken);

    Task HandleDeliveryStartedAsync(OrderDeliveryStartedEvent deliveryEvent, CancellationToken cancellationToken);

    Task HandleDeliveryFinishedAsync(OrderDeliveryFinishedEvent deliveryEvent, CancellationToken cancellationToken);
}