using BL.DTOs;

namespace PV293WebApplication.Models;

public class HomeIndexViewModel
{
    public List<CategoryDTO> PopularCategories { get; set; } = new();
    public List<ProductDTO> FeaturedProducts { get; set; } = new();
}
