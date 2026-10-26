using System.ComponentModel.DataAnnotations;
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
        [FromQuery, Range(1, int.MaxValue)] int page = Pagination.DefaultPage,
        [FromQuery, Range(1, Pagination.MaximumPageSize)] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var result = await productService.GetProductsAsync(page, pageSize, cancellationToken);

        return Ok(result);
    }
}
