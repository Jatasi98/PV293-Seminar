using MediatR;
using VerticalSlice.Domain.Entities;

namespace VerticalSlice.Features.Product.CreateProduct;

public class CreateProductCommand : IRequest<int>
{
    public string Name { get; set; } = default!;

    public Money Price { get; set; } = default!;

    public string? Description { get; set; }

    public int? CategoryId { get; set; }
}
