using Interview.API.Models;
using Interview.Application.DTOs;
using Interview.Application.Interfaces;
using Interview.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Interview.API.Controllers;

[ApiController]
[Route("products")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<ProductDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] GetProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new ProductQuery(request.Search, request.Page, request.PageSize);
        var result = await productService.GetProductsAsync(query, cancellationToken);

        return Ok(result);
    }
}
