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

        // Manual validation here to demonstrate understanding without implementing a lib of some kind
        // Necessary because the the service layer is decoupled from the API layer, and could be used in other areas without API layer validation
        // Executed first to avoid unnecessary compute 
        ArgumentNullException.ThrowIfNull(query);
        ArgumentOutOfRangeException.ThrowIfLessThan(query.Page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(query.PageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(query.PageSize, Pagination.MaximumPageSize);

        // Bit of sanitisation
        var search = string.IsNullOrWhiteSpace(query.Search)
            ? null
            : query.Search.Trim();

        // Search query validation using the shared Const
        if (search?.Length > ProductQuery.MaximumSearchLength)
        {
            throw new ArgumentException(
                $"Search cannot exceed {ProductQuery.MaximumSearchLength} characters.",
                nameof(query));
        }

        // "with" lets me pass the query here with the sanitised search value.
        // It passes a new query because I haven't explicitly declared the set accessor, the Record is declared with init by detault making it immutable.
        // The original record is still available here for "stuff".
        // Just leaning into the understanding of the principle that Types / States should generally be closed by default -
        // and explicitly opened for extension / mutation when needed.
        var result = await repository.GetPageAsync(
            query with { Search = search },
            cancellationToken);

        // Standard manual DTO mapping here, usually handled by the likes of the - now controversial - AutoMapper! 
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
