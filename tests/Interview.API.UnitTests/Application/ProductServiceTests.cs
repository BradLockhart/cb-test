using Interview.Application.Interfaces;
using Interview.Application.Models;
using Interview.Application.Services;
using Interview.Domain.Entities;
using Xunit;

namespace Interview.API.UnitTests.Application;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task GetProductsAsync_MapsProductsAndPagination()
    {
        var category = new Category
        {
            CategoryId = Guid.NewGuid(),
            Name = "Graphics Cards",
            IsActive = true
        };

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Example GPU",
            Sku = "GPU-001",
            IsActive = true,
            CategoryId = category.CategoryId,
            Category = category
        };

        var repository = new FakeProductRepository(
            new PagedResult<Product>([product], 2, 5, 11));

        var service = new ProductService(repository);

        using var cancellationSource = new CancellationTokenSource();

        var result = await service.GetProductsAsync(
            new ProductQuery("  gpu  ", 2, 5),
            cancellationSource.Token);

        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(11, result.TotalItems);
        Assert.Equal(3, result.TotalPages);

        var item = Assert.Single(result.Items);
        Assert.Equal(product.Id, item.Id);
        Assert.Equal(product.Name, item.Name);
        Assert.Equal(product.Sku, item.Sku);
        Assert.True(item.IsActive);
        Assert.NotNull(item.Category);
        Assert.Equal(category.CategoryId, item.Category.Id);
        Assert.Equal(category.Name, item.Category.Name);

        Assert.Equal("gpu", repository.Query?.Search);
        Assert.Equal(2, repository.Query?.Page);
        Assert.Equal(5, repository.Query?.PageSize);
        Assert.Equal(cancellationSource.Token, repository.CancellationToken);
    }

    [Fact]
    public async Task GetProductsAsync_MapsMissingCategoryToNull()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Example Peripheral",
            Sku = "PER-001",
            IsActive = false
        };

        var repository = new FakeProductRepository(
            new PagedResult<Product>([product], 1, 10, 1));

        var service = new ProductService(repository);

        var result = await service.GetProductsAsync(new ProductQuery(null, 1, 10));

        var item = Assert.Single(result.Items);
        Assert.Null(item.Category);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(1, Pagination.MaximumPageSize + 1)]
    public async Task GetProductsAsync_WhenPaginationIsInvalid_Throws(
        int page,
        int pageSize)
    {
        var repository = new FakeProductRepository(
            new PagedResult<Product>([], 1, 10, 0));

        var service = new ProductService(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetProductsAsync(new ProductQuery(null, page, pageSize)));

        Assert.False(repository.WasCalled);
    }

    [Fact]
    public async Task GetProductsAsync_WhenSearchIsTooLong_Throws()
    {
        var repository = new FakeProductRepository(
            new PagedResult<Product>([], 1, 10, 0));
        var service = new ProductService(repository);
        var search = new string('a', ProductQuery.MaximumSearchLength + 1);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetProductsAsync(new ProductQuery(search, 1, 10)));

        Assert.False(repository.WasCalled);
    }

    private sealed class FakeProductRepository(PagedResult<Product> result)
        : IProductRepository
    {
        public bool WasCalled { get; private set; }
        public ProductQuery? Query { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        public Task<PagedResult<Product>> GetPageAsync(
            ProductQuery query,
            CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            Query = query;
            CancellationToken = cancellationToken;

            return Task.FromResult(result);
        }
    }
}
