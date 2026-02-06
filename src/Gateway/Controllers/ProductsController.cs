using Gateway.Clients.GrpcProducts;
using Gateway.Models.Requests;
using Gateway.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductGrpcClient _productClient;

    public ProductsController(IProductGrpcClient productClient)
    {
        _productClient = productClient;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create new products")]
    [SwaggerResponse(StatusCodes.Status201Created, "Products created successfully", typeof(ProductListResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error", typeof(ErrorResponse))]
    public async Task<ActionResult<ProductListResponse>> CreateProducts(
        [FromBody] CreateProductsRequest request,
        CancellationToken cancellationToken)
    {
        ProductListResponse response = await _productClient.CreateProductsAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}