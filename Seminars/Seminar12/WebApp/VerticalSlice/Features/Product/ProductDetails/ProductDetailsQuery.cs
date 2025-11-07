using MediatR;
using VerticalSlice.DTOs;

namespace VerticalSlice.Features.Product.ProductDetails;

public class ProductDetailsQuery : IRequest<ProductDTO>
{
    public int Id { get; internal set; }
}
