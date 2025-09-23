using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Entities;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "CART";
        private readonly BaseRepository _db;

        public CartController(BaseRepository db) => _db = db;

        public IActionResult Index()
        {
            var vm = GetCart();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, int qty = 1)
        {
            if (qty < 1) qty = 1;

            var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == id);
            if (item == null)
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    UnitPrice = product.Price,
                    Quantity = qty
                });
            }
            else
            {
                item.Quantity += qty;
            }

            SaveCart(cart);
            TempData["CartMessage"] = $"{product.Name} added to cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, int qty)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == id);
            if (item == null) return NotFound();

            if (qty <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = qty;

            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            cart.Items.RemoveAll(i => i.ProductId == id);
            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            SaveCart(new CartViewModel());
            return RedirectToAction(nameof(Index));
        }

        private CartViewModel GetCart()
            => HttpContext.Session.GetObject<CartViewModel>(CartKey) ?? new CartViewModel();

        private void SaveCart(CartViewModel cart)
            => HttpContext.Session.SetObject(CartKey, cart);
    }
}
