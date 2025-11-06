using Microsoft.AspNetCore.Mvc;

namespace VerticalSlice.Features.Cart.GetCart;

public class GetCartController : CartControllerBase
{
    public IActionResult Index()
    {
        var cart = GetCart();

        return View(cart);
    }
}
