namespace BL.DTOs;

public class SearchResult
{
    public string Query { get; set; } = string.Empty;
    public PagedResult<ProductDTO> Products { get; set; }
    public IReadOnlyList<CategoryDTO> Categories { get; set; } = [];
}
