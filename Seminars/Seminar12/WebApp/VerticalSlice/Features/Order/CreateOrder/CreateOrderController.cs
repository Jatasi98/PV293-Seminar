using MediatR;
using Microsoft.AspNetCore.Mvc;
using Module.Orders.Application.DTOs;
using Module.Orders.Application.Features.CreateOrder;
using VerticalSlice.Features.Cart;

namespace VerticalSlice.Features.Order.CreateOrder;

[ApiController]
[Route("api/[controller]")]
public sealed class CreateOrderController(IMediator mediator) : CartControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var cart = GetCart();

        var command = new CreateOrderCommand(new() { Id = cart.CustomerId }, cart.Items.Select(x => new OrderItemDTO() { ProductId = x.ProductId, ProductName = x.ProductName, Quantity = x.Quantity, UnitPrice = x.UnitPrice }));

        var id = await _mediator.Send(command);

        return Ok(new { OrderId = id });
    }
}
