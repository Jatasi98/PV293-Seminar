using MediatR;
using Microsoft.AspNetCore.Mvc;
using VerticalSlice.DTOs;

namespace VerticalSlice.Features.Cart.AddCartItem;

public class AddCartItemController(IMediator mediator) : CartControllerBase
{
    private readonly IMediator mediator = mediator;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id, int qty = 1)
    {
        if (qty < 1)
        {
            qty = 1;
        }

        var product = await mediator.Send(new AddCartItemQuery() { Id = id });

        if (product == null)
        {
            return NotFound();
        }

        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == id);

        if (item == null)
        {
            cart.Items.Add(new CartItemDTO
            {
                ProductId = product.Id,
                UnitPrice = product.Price,
                Quantity = qty,
            });
        }
        else
        {
            item.Quantity += qty;
        }

        SaveCart(cart);

        return RedirectToCart();
    }
}
