// Gateway/Controllers/OrderProcessingController.cs
using Gateway.Clients.GrpcOrderProcessing;
using Gateway.Models.Requests;
using Gateway.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Gateway.Controllers;

[ApiController]
[Route("api/orders/{orderId:long}/processing")]
public class OrderProcessingController : ControllerBase
{
    private readonly IOrderProcessingGrpcClient _processingClient;

    public OrderProcessingController(IOrderProcessingGrpcClient processingClient)
    {
        _processingClient = processingClient;
    }

    [HttpPost("approve")]
    [SwaggerOperation(Summary = "Approve or reject an order")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Order approval processed")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Invalid order state", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> ApproveOrder(
        [FromRoute] long orderId,
        [FromBody] ApproveOrderRequest request,
        CancellationToken cancellationToken)
    {
        await _processingClient.ApproveOrderAsync(orderId, request, cancellationToken);

        return NoContent();
    }

    [HttpPost("packing/start")]
    [SwaggerOperation(Summary = "Start packing an order")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Packing started")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Invalid order state", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> StartPacking(
        [FromRoute] long orderId,
        [FromBody] StartPackingRequest request,
        CancellationToken cancellationToken)
    {
        await _processingClient.StartPackingAsync(orderId, request, cancellationToken);

        return NoContent();
    }

    [HttpPost("packing/finish")]
    [SwaggerOperation(Summary = "Finish packing an order")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Packing finished")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Invalid order state", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> FinishPacking(
        [FromRoute] long orderId,
        [FromBody] FinishPackingRequest request,
        CancellationToken cancellationToken)
    {
        await _processingClient.FinishPackingAsync(orderId, request, cancellationToken);

        return NoContent();
    }

    [HttpPost("delivery/start")]
    [SwaggerOperation(Summary = "Start delivery of an order")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Delivery started")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Invalid order state", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> StartDelivery(
        [FromRoute] long orderId,
        [FromBody] StartDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        await _processingClient.StartDeliveryAsync(orderId, request, cancellationToken);

        return NoContent();
    }

    [HttpPost("delivery/finish")]
    [SwaggerOperation(Summary = "Finish delivery of an order")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Delivery finished")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Invalid order state", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> FinishDelivery(
        [FromRoute] long orderId,
        [FromBody] FinishDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        await _processingClient.FinishDeliveryAsync(orderId, request, cancellationToken);

        return NoContent();
    }
}