using Microsoft.AspNetCore.Mvc;

namespace VerticalSlice.Features.Cart.UpdateCartItem;

public class UpdateCartItemController : CartControllerBase
{
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

        return RedirectToCart();
    }
}
