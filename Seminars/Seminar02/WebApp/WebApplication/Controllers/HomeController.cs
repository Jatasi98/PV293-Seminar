using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BaseRepository _db;

        public HomeController(ILogger<HomeController> logger, BaseRepository db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var cats = await _db.Categories
                .OrderBy(c => c.Id)
                .Take(4)
                .AsNoTracking()
                .ToListAsync();

            var prods = await _db.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedOnUTC)
                .Take(8)
                .AsNoTracking()
                .ToListAsync();

            var vm = new HomeIndexViewModel
            {
                PopularCategories = cats,
                FeaturedProducts = prods
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
