namespace Interview.Application.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    bool IsActive,
    CategoryDto? Category);

public sealed record CategoryDto(Guid Id, string Name);
