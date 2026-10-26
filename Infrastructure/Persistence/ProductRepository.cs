using Interview.Application.Interfaces;
using Interview.Application.Models;
using Interview.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class ProductRepository(InterviewDbContext context) : IProductRepository
{
    public async Task<PagedResult<Product>> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var products = context.Products
            .AsNoTracking()
            .Include(product => product.Category);

        var totalItems = await products.CountAsync(cancellationToken);

        var items = await products
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(
            items,
            page,
            pageSize,
            totalItems);
    }
}
