using Interview.API.Controllers;
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

        var response = await controller.GetProducts(2, 5, cancellationSource.Token);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);

        Assert.Same(expected, okResult.Value);
        Assert.Equal(2, service.RequestedPage);
        Assert.Equal(5, service.RequestedPageSize);
        Assert.Equal(cancellationSource.Token, service.CancellationToken);
    }

    private sealed class FakeProductService(PagedResult<ProductDto> result) : IProductService
    {
        public int? RequestedPage { get; private set; }
        public int? RequestedPageSize { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        public Task<PagedResult<ProductDto>> GetProductsAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            RequestedPage = page;
            RequestedPageSize = pageSize;
            CancellationToken = cancellationToken;

            return Task.FromResult(result);
        }
    }
}
