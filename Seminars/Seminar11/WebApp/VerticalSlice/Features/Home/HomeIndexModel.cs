using VerticalSlice.DTOs;

namespace VerticalSlice.Features.Home;

public class HomeIndexModel
{
    public List<CategoryDTO> PopularCategories { get; set; } = [];
    public List<ProductDTO> FeaturedProducts { get; set; } = [];
}
