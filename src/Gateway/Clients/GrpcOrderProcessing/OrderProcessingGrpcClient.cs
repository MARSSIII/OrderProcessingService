using GrpcContracts = Orders.ProcessingService.Contracts;
using HttpRequests = Gateway.Models.Requests;

namespace Gateway.Clients.GrpcOrderProcessing;

public class OrderProcessingGrpcClient : IOrderProcessingGrpcClient
{
    private readonly GrpcContracts.OrderService.OrderServiceClient _client;

    public OrderProcessingGrpcClient(GrpcContracts.OrderService.OrderServiceClient client)
    {
        _client = client;
    }

    public async Task ApproveOrderAsync(
        long orderId,
        HttpRequests.ApproveOrderRequest request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new GrpcContracts.ApproveOrderRequest
        {
            OrderId = orderId,
            IsApproved = request.IsApproved,
            ApprovedBy = request.ApprovedBy,
        };

        if (request.FailureReason is not null)
        {
            grpcRequest.FailureReason = request.FailureReason;
        }

        await _client.ApproveOrderAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task StartPackingAsync(
        long orderId,
        HttpRequests.StartPackingRequest request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new GrpcContracts.StartOrderPackingRequest
        {
            OrderId = orderId,
            PackingBy = request.PackingBy,
        };

        await _client.StartOrderPackingAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task FinishPackingAsync(
        long orderId,
        HttpRequests.FinishPackingRequest request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new GrpcContracts.FinishOrderPackingRequest
        {
            OrderId = orderId,
            IsSuccessful = request.IsSuccessful,
        };

        if (request.FailureReason is not null)
        {
            grpcRequest.FailureReason = request.FailureReason;
        }

        await _client.FinishOrderPackingAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task StartDeliveryAsync(
        long orderId,
        HttpRequests.StartDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new GrpcContracts.StartOrderDeliveryRequest
        {
            OrderId = orderId,
            DeliveredBy = request.DeliveredBy,
        };

        await _client.StartOrderDeliveryAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    public async Task FinishDeliveryAsync(
        long orderId,
        HttpRequests.FinishDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var grpcRequest = new GrpcContracts.FinishOrderDeliveryRequest
        {
            OrderId = orderId,
            IsSuccessful = request.IsSuccessful,
        };

        if (request.FailureReason is not null)
        {
            grpcRequest.FailureReason = request.FailureReason;
        }

        await _client.FinishOrderDeliveryAsync(grpcRequest, cancellationToken: cancellationToken);
    }
}