using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Entities;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CheckoutController : Controller
    {
        private const string CartKey = "CART";
        private readonly BaseRepository _db;
        private readonly UserManager<AppUser> _userManager;

        public CheckoutController(BaseRepository db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<CartViewModel>(CartKey) ?? new CartViewModel();
            if (!cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            var vm = new CheckoutViewModel { Cart = cart };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel vm)
        {
            var appUserId = _userManager.GetUserId(User);

            var customerId = await _db.Customers
                .Where(x => x.AppUserId == appUserId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (customerId == 0)
            {
                ModelState.AddModelError("", "Unable to resolve customer.");
                vm.Cart = new CartViewModel();
                return View("Index", vm);
            }

            // 2) Load the persisted cart
            var cartEntity = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cartEntity == null || !cartEntity.Items.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                vm.Cart = new CartViewModel();
                return View("Index", vm);
            }

            if (!ModelState.IsValid)
            {
                vm.Cart = new CartViewModel
                {
                    Items = cartEntity.Items.Select(i => new CartItem
                    {
                        ProductId = i.ProductId,
                        Name = i.Name,
                        UnitPrice = i.UnitPrice,
                        Quantity = i.Quantity
                    }).ToList()
                };
                return View("Index", vm);
            }

            var productIds = cartEntity.Items.Select(i => i.ProductId).ToList();
            var products = await _db.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var i in cartEntity.Items)
            {
                if (!products.TryGetValue(i.ProductId, out var p))
                {
                    ModelState.AddModelError("", $"Product {i.ProductId} no longer available.");
                    return View("Index", vm);
                }
                i.Name = p.Name;
                i.UnitPrice = p.Price;
            }

            var subtotal = cartEntity.Items.Sum(i => i.UnitPrice * i.Quantity);
            var shipping = cartEntity.Items.Count > 0 ? 5.00m : 0m;
            var total = subtotal + shipping;

            var order = new Order
            {
                CustomerId = customerId,
                CreatedOnUTC = DateTime.UtcNow,
                Total = total,
                FullName = vm.FullName,
                Address1 = vm.Address1,
                Address2 = vm.Address2,
                City = vm.City,
                Zip = vm.Zip,
                Country = vm.Country,
                Items = cartEntity.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.Name,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };

            _db.Orders.Add(order);

            _db.Carts.Remove(cartEntity);

            await _db.SaveChangesAsync();

            HttpContext.Session.Remove("CART");

            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }
    }
}
