using Interview.Application.Interfaces;
using Interview.Application.Models;
using Interview.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class ProductRepository(InterviewDbContext context) : IProductRepository
{
    public async Task<PagedResult<Product>> GetPageAsync(
        ProductQuery query,
        CancellationToken cancellationToken = default)
    {

        // IQueryable to let me build up the queryand add filters before execution.
        // Eager load categories, I know I always need them for every Product
        // No tracking because it's read only, no need to change track.
        IQueryable<Product> products = context.Products
            .AsNoTracking()
            .Include(product => product.Category);


        // If I have a search term, filter the products so I'm only returning products that match the search term.
        if (query.Search is not null)
        {
            var search = query.Search.ToLowerInvariant();

            products = products.Where(product =>
                product.Name.ToLower().Contains(search) ||
                product.Sku.ToLower().Contains(search));
        }

        var totalItems = await products.CountAsync(cancellationToken);


        // Very simple offset pagination here - 
        // PageSize is how many products to return
        // Page is which page of products to return, so I can skip the previous pages and take the next page.

        // PageSize = 5
        // Page 1: skip 0, take 5
        // Page 2: skip 5, take 5
        // Page 3: skip 10, take 5
        // and so on.
        var items = await products
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        // Return that useful Generic with a different type of model, good stuff.
        return new PagedResult<Product>(
            items,
            query.Page,
            query.PageSize,
            totalItems);
    }
}
