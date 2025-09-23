using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Entities;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly BaseRepository _db;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(BaseRepository db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var appUserId = _userManager.GetUserId(User);

            var customerId = await _db.Customers
                .Where(c => c.AppUserId == appUserId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (customerId == 0)
            {
                return View(new List<Order>());
            }

            var orders = await _db.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedOnUTC)
                .AsNoTracking()
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appUserId = _userManager.GetUserId(User);
            var customerId = await _db.Customers
                .Where(c => c.AppUserId == appUserId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            var order = await _db.Orders
                .Include(o => o.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id && (customerId == 0 || o.CustomerId == customerId));

            if (order == null) return NotFound();

            return View(order);
        }
    }
}
