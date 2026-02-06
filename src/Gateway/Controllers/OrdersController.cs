using Gateway.Clients.GrpcOrders;
using Gateway.Models.Requests;
using Gateway.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderGrpcClient _orderClient;

    public OrdersController(IOrderGrpcClient orderClient)
    {
        _orderClient = orderClient;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new order")]
    [SwaggerResponse(StatusCodes.Status201Created, "Order created successfully", typeof(OrderResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        OrderResponse response = await _orderClient.CreateOrderAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetOrderHistory), new { orderId = response.Id }, response);
    }

    [HttpPost("{orderId:long}/items")]
    [SwaggerOperation(Summary = "Add items to an existing order")]
    [SwaggerResponse(StatusCodes.Status200OK, "Items added successfully", typeof(OrderItemListResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<ActionResult<OrderItemListResponse>> AddItemsToOrder(
        [FromRoute] long orderId,
        [FromBody] AddItemsToOrderRequest request,
        CancellationToken cancellationToken)
    {
        OrderItemListResponse response = await _orderClient.AddItemsToOrderAsync(orderId, request, cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{orderId:long}/items")]
    [SwaggerOperation(Summary = "Delete item from order")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Item deleted successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> DeleteItemInOrder(
        [FromRoute] long orderId,
        [FromQuery] long productId,
        CancellationToken cancellationToken)
    {
        await _orderClient.DeleteItemInOrderAsync(orderId, productId, cancellationToken);

        return NoContent();
    }

    [HttpPost("{orderId:long}/transfer-to-work")]
    [SwaggerOperation(Summary = "Transfer order to processing state")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Order transferred to processing")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid state transition", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Invalid order state", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> TransferToWork(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        await _orderClient.TransferToWorkAsync(orderId, cancellationToken);

        return NoContent();
    }

    [HttpPost("{orderId:long}/cancel")]
    [SwaggerOperation(Summary = "Cancel the order")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Order cancelled successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid state transition", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Invalid order state", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> CancelOrder(
        [FromRoute] long orderId,
        CancellationToken cancellationToken)
    {
        await _orderClient.CancelOrderAsync(orderId, cancellationToken);

        return NoContent();
    }

    [HttpGet("{orderId:long}/history")]
    [SwaggerOperation(Summary = "Get order history with pagination")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order history retrieved", typeof(OrderHistoryListResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<ActionResult<OrderHistoryListResponse>> GetOrderHistory(
        [FromRoute] long orderId,
        [FromQuery] long cursor = 0,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        OrderHistoryListResponse response = await _orderClient.GetOrderHistoryAsync(orderId, cursor, pageSize, cancellationToken);

        return Ok(response);
    }
}
