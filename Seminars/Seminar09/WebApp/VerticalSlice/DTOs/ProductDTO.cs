namespace VerticalSlice.DTOs;

public class ProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
