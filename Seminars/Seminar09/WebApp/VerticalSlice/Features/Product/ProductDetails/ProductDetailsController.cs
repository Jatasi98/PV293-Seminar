using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VerticalSlice.Features.Product.ProductDetails;

public class ProductDetailsController(IMediator mediator) : Controller
{
    private readonly IMediator mediator = mediator;

    public async Task<IActionResult> Details(int id)
    {
        var product = await mediator.Send(new ProductDetailsQuery()
        {
            Id = id,
        });

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}
