namespace Application.DTOs;

public class SearchResult
{
    public string Query { get; set; } = string.Empty;

    public PagedResult<ProductDTO> Products { get; set; } = default!;

    public IReadOnlyList<CategoryDTO> Categories { get; set; } = [];
}
