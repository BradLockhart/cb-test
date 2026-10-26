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
        IQueryable<Product> products = context.Products
            .AsNoTracking()
            .Include(product => product.Category);

        if (query.Search is not null)
        {
            var search = query.Search.ToLowerInvariant();

            products = products.Where(product =>
                product.Name.ToLower().Contains(search) ||
                product.Sku.ToLower().Contains(search));
        }

        var totalItems = await products.CountAsync(cancellationToken);

        var items = await products
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(
            items,
            query.Page,
            query.PageSize,
            totalItems);
    }
}
