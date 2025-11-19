using VerticalSlice.DTOs;

namespace VerticalSlice.Features.Search.GetSearch;

public class GetSearchModel
{
    public string Query { get; set; } = string.Empty;
    public List<ProductDTO> Products { get; set; } = [];
    public List<CategoryDTO> Categories { get; set; } = [];
}
