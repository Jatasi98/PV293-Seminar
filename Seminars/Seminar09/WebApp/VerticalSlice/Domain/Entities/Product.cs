namespace VerticalSlice.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public bool IsDeleted { get; set; } = false;

    public int? CategoryId { get; set; }

    public Category? Category { get; set; }
}
