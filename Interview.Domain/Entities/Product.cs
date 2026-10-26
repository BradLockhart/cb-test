namespace Interview.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public bool IsActive { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
}
