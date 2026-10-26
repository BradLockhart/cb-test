namespace Interview.Application.Models;

public sealed record ProductQuery(string? Search, int Page, int PageSize)
{
    public const int MaximumSearchLength = 100;
}
