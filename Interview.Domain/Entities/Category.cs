namespace Interview.Domain.Entities;

public sealed class Category
{
    public Guid CategoryId { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
}
