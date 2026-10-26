using Interview.Application.DTOs;
using Interview.Application.Models;

namespace Interview.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
