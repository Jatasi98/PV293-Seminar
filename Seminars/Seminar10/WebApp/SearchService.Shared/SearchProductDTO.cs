namespace SearchService.Shared;

public sealed class SearchProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? CategoryName { get; set; }
}
