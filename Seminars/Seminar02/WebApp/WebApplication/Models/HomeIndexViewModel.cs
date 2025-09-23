using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class HomeIndexViewModel
    {
        public List<Category> PopularCategories { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
    }
}
