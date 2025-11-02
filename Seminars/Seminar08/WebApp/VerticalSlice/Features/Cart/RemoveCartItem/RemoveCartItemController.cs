using Microsoft.AspNetCore.Mvc;

namespace VerticalSlice.Features.Cart.RemoveCartItem;

public class RemoveCartItemController : CartControllerBase
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        var cart = GetCart();

        cart.Items.RemoveAll(i => i.ProductId == id);

        SaveCart(cart);

        return RedirectToCart();
    }
}
