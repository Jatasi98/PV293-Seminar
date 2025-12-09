using MediatR;
using VerticalSlice.DTOs;

namespace VerticalSlice.Features.Cart.AddCartItem;

public class AddCartItemQuery : IRequest<ProductDTO?>
{
    public int Id { get; set; }
}
