using MediatR;
using VerticalSlice.Domain.Events;
using VerticalSlice.DTOs;
using VerticalSlice.Services;

namespace VerticalSlice.Features.Cart.AddCartItem;

public class AddCartItemHandler(IProductService productService, IMediator mediator) : IRequestHandler<AddCartItemQuery, ProductDTO?>
{
    private readonly IProductService productService = productService;
    private readonly IMediator mediator = mediator;

    public async Task<ProductDTO?> Handle(AddCartItemQuery request, CancellationToken cancellationToken)
    {
        var product = await productService.GetProduct(request.Id);
        
        if (product != null)
        {
            await mediator.Publish(new ProductAddedToCartEvent(product.Id));
        }

        return product;
    }
}
