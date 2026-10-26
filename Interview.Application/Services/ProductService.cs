using Interview.Application.DTOs;
using Interview.Application.Interfaces;
using Interview.Application.Models;

namespace Interview.Application.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, Pagination.MaximumPageSize);

        var result = await repository.GetPageAsync(page, pageSize, cancellationToken);

        var items = result.Items
            .Select(product => new ProductDto(
                product.Id,
                product.Name,
                product.Sku,
                product.IsActive,
                product.Category is null
                    ? null
                    : new CategoryDto(product.Category.CategoryId, product.Category.Name)))
            .ToArray();

        return new PagedResult<ProductDto>(
            items,
            result.Page,
            result.PageSize,
            result.TotalItems);
    }
}
