using Application.DTOs;

namespace Presentation.Models;

public class HomeIndexViewModel
{
    public List<CategoryDTO> PopularCategories { get; set; } = new();
    public List<ProductDTO> FeaturedProducts { get; set; } = new();
}
