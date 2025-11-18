using Microsoft.AspNetCore.Mvc;
using VerticalSlice.Domain;
using VerticalSlice.Features.Cart.Models;
using VerticalSlice.Helpers;

namespace VerticalSlice.Features.Cart;

public class CartControllerBase : Controller
{
    public CartModel GetCart()
        => HttpContext.Session.GetObject<CartModel>(Constants.CartKey) ?? new CartModel();

    public void SaveCart(CartModel cart)
        => HttpContext.Session.SetObject(Constants.CartKey, cart);

    public IActionResult RedirectToCart()
        => RedirectToAction("Index", "GetCart");
}
