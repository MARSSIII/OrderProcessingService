using Gateway.Models.Requests;

namespace Gateway.Clients.GrpcOrderProcessing;

public interface IOrderProcessingGrpcClient
{
    Task ApproveOrderAsync(long orderId, ApproveOrderRequest request, CancellationToken cancellationToken);

    Task StartPackingAsync(long orderId, StartPackingRequest request, CancellationToken cancellationToken);

    Task FinishPackingAsync(long orderId, FinishPackingRequest request, CancellationToken cancellationToken);

    Task StartDeliveryAsync(long orderId, StartDeliveryRequest request, CancellationToken cancellationToken);

    Task FinishDeliveryAsync(long orderId, FinishDeliveryRequest request, CancellationToken cancellationToken);
}