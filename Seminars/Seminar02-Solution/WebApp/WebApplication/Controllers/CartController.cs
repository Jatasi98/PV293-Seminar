using BL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using PV293WebApplication.Helpers;
using PV293WebApplication.Models;
using Shared;

namespace PV293WebApplication.Controllers;

public class CartController : Controller
{
    private readonly IProductService _productService;

    public CartController(IProductService productService) => _productService = productService;

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

        var product = await _productService.GetProduct(id);

        if (product == null)
        {
            return NotFound();
        }

        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == id);

        if (item == null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
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

        if (item == null)
        { 
            return NotFound();
        }

        if (qty <= 0)
        {
            cart.Items.Remove(item);
        }            
        else
        {
            item.Quantity = qty;
        }            

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
        => HttpContext.Session.GetObject<CartViewModel>(Constants.CartKey) ?? new CartViewModel();

    private void SaveCart(CartViewModel cart)
        => HttpContext.Session.SetObject(Constants.CartKey, cart);
}
