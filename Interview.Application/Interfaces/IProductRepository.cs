using Interview.Application.Models;
using Interview.Domain.Entities;

namespace Interview.Application.Interfaces;

public interface IProductRepository
{
    Task<PagedResult<Product>> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
