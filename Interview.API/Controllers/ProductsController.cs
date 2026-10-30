using Interview.API.Models;
using Interview.Application.DTOs;
using Interview.Application.Interfaces;
using Interview.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Interview.API.Controllers;

/*
In recent times I've been interested in exploring different architectures -
including vertical slice architecture, and mapping isolated through IEndpointRouteBuilder as minimal APIs -
to give a slice full ownershipof its use case, including its endpoint.

I could have used a minimal API here, but for this test I decided on the resource-based approach for no other reason than -
the job spec mentioned Clean Architecture, and this is the more practical and convenient pairing that felt familiar to me in the Clean Architecture style.
*/
[ApiController]
[Route("products")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<ProductDto>>(StatusCodes.Status200OK)]

    // 400 is a designed response for when the request fails validation, so it's worth documenting it here for swagger.
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]

    // A Generic (PagedResult<T>) is good here as it's reuseable for any type of model used for paginated results - 
    // which could all typically reuse page, pageSize, totalItems, and totalPages. 
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        // Request model Class as a clean way to handle API level validation and keep the controller tidy using query binding.
        [FromQuery] GetProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        // Again, just a demonstration of clean programming practice, a query model Record to keep the service layer clean and decoupled from the API layer
        // this could be extended later without needing to change the service layer params
        var query = new ProductQuery(request.Search, request.Page, request.PageSize);

        var result = await productService.GetProductsAsync(query, cancellationToken);

        return Ok(result);
    }
}
