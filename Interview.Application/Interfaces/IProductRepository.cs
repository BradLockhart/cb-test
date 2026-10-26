using Interview.Application.Models;
using Interview.Domain.Entities;

namespace Interview.Application.Interfaces;

public interface IProductRepository
{
    Task<PagedResult<Product>> GetPageAsync(
        ProductQuery query,
        CancellationToken cancellationToken = default);
}
