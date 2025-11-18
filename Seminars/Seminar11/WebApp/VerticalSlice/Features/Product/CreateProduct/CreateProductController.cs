using MediatR;
using Microsoft.AspNetCore.Mvc;
using VerticalSlice.Domain.Entities;
using VerticalSlice.DTOs;

namespace VerticalSlice.Features.Product.CreateProduct;

public class CreateProductController(IMediator mediator) : Controller
{
    private readonly IMediator mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductDTO newProduct)
    {
        var productId = await mediator.Send(new CreateProductCommand()
        {
            Name = newProduct.Name,
            Description = newProduct.Description,
            CategoryId = newProduct.CategoryId,
            Price = Money.Create(newProduct.Price, newProduct.Currency),
        });

        return RedirectToAction("Details", "ProductDetails", new { id = productId });
    }
}
