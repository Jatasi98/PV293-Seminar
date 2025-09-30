using DAL.Entities;

namespace PV293WebApplication.Models;

public class SearchViewModel
{
    public string Query { get; set; } = "";
    public List<Product> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
}
