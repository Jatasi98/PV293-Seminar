using BL.DTOs;

namespace PV293WebApplication.Models;

public class SearchViewModel
{
    public string Query { get; set; } = "";
    public List<ProductDTO> Products { get; set; } = new();
    public List<CategoryDTO> Categories { get; set; } = new();
}
