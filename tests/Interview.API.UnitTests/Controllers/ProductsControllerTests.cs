using Interview.API.Controllers;
using Interview.API.Models;
using Interview.Application.DTOs;
using Interview.Application.Interfaces;
using Interview.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Interview.API.UnitTests.Controllers;

public sealed class ProductsControllerTests
{
    [Fact]
    public async Task GetProducts_ReturnsServiceResult()
    {
        var expected = new PagedResult<ProductDto>(
            [new ProductDto(Guid.NewGuid(), "Example GPU", "GPU-001", true, null)],
            2,
            5,
            7);

        var service = new FakeProductService(expected);

        var controller = new ProductsController(service);

        using var cancellationSource = new CancellationTokenSource();

        var request = new GetProductsRequest
        {
            Search = "GPU",
            Page = 2,
            PageSize = 5
        };

        var response = await controller.GetProducts(request, cancellationSource.Token);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);

        Assert.Same(expected, okResult.Value);
        Assert.Equal("GPU", service.Query?.Search);
        Assert.Equal(2, service.Query?.Page);
        Assert.Equal(5, service.Query?.PageSize);
        Assert.Equal(cancellationSource.Token, service.CancellationToken);
    }

    private sealed class FakeProductService(PagedResult<ProductDto> result) : IProductService
    {
        public ProductQuery? Query { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        public Task<PagedResult<ProductDto>> GetProductsAsync(
            ProductQuery query,
            CancellationToken cancellationToken = default)
        {
            Query = query;
            CancellationToken = cancellationToken;

            return Task.FromResult(result);
        }
    }
}
