using System.ComponentModel.DataAnnotations;
using Interview.Application.Models;

namespace Interview.API.Models;

public sealed class GetProductsRequest
{
    [StringLength(ProductQuery.MaximumSearchLength)]
    public string? Search { get; init; }

    [Range(1, int.MaxValue)]
    public int Page { get; init; } = Pagination.DefaultPage;

    [Range(1, Pagination.MaximumPageSize)]
    public int PageSize { get; init; } = Pagination.DefaultPageSize;
}
