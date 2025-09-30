using DAL.Entities;

namespace PV293WebApplication.Models;

public class HomeIndexViewModel
{
    public List<Category> PopularCategories { get; set; } = new();
    public List<Product> FeaturedProducts { get; set; } = new();
}
