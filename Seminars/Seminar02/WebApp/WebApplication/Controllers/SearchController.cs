using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Entities;

namespace WebApplication1.Controllers
{
    public class SearchController : Controller
    {
        private readonly BaseRepository _db;
        public SearchController(BaseRepository db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Index(string? q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View(new SearchViewModel
                {
                    Query = "",
                    Products = new List<Product>(),
                    Categories = new List<Category>()
                });
            }

            q = q.Trim();

            var products = await _db.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.Name.Contains(q) || (p.Description != null && p.Description.Contains(q)))
                .ToListAsync();

            var categories = await _db.Categories
                .AsNoTracking()
                .Where(c => c.Name.Contains(q) || (c.Description != null && c.Description.Contains(q)))
                .ToListAsync();

            var vm = new SearchViewModel
            {
                Query = q,
                Products = products,
                Categories = categories
            };

            return View(vm);
        }
    }

    public class SearchViewModel
    {
        public string Query { get; set; } = "";
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
}
