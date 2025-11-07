using MediatR;
using VerticalSlice.DTOs;
using VerticalSlice.Services;

namespace VerticalSlice.Features.Product.ProductDetails;

public class ProductDetailsHandler(IProductService productService) : IRequestHandler<ProductDetailsQuery, ProductDTO>
{
    private readonly IProductService productService = productService;

    public async Task<ProductDTO> Handle(ProductDetailsQuery request, CancellationToken cancellationToken)
    {
        var product = await productService.GetProduct(request.Id);

        if (product == null)
        {
            throw new Exception("Product does not exist");
        }

        return product;
    }
}
