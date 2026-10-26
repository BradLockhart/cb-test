using Interview.Application.DTOs;
using Interview.Application.Models;

namespace Interview.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(
        ProductQuery query,
        CancellationToken cancellationToken = default);
}
