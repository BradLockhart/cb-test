using Interview.Application.DTOs;
using Interview.Application.Interfaces;
using Interview.Application.Models;

namespace Interview.Application.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        ProductQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentOutOfRangeException.ThrowIfLessThan(query.Page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(query.PageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(query.PageSize, Pagination.MaximumPageSize);

        var search = string.IsNullOrWhiteSpace(query.Search)
            ? null
            : query.Search.Trim();

        if (search?.Length > ProductQuery.MaximumSearchLength)
        {
            throw new ArgumentException(
                $"Search cannot exceed {ProductQuery.MaximumSearchLength} characters.",
                nameof(query));
        }

        var result = await repository.GetPageAsync(
            query with { Search = search },
            cancellationToken);

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
