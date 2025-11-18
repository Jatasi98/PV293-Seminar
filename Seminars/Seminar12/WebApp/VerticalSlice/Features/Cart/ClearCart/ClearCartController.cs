using Microsoft.AspNetCore.Mvc;
using VerticalSlice.Features.Cart.Models;

namespace VerticalSlice.Features.Cart.ClearCart;

public class ClearCartController : CartControllerBase
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        SaveCart(new CartModel());

        return RedirectToCart();
    }
}
