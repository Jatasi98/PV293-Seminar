using MediatR;
using SearchService.Shared;
using VerticalSlice.Infrastructure;
using Wolverine;

namespace VerticalSlice.Features.Product.CreateProduct;

public class CreateProductHandler(WebAppDbContext db, IMessageBus bus) : IRequestHandler<CreateProductCommand, int>
{
    private readonly WebAppDbContext db = db;
    private readonly IMessageBus bus = bus;

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Domain.Entities.Product.Create(request.Name, request.Description, request.Price, request.CategoryId);
        db.Products.Add(product);

        await db.SaveChangesAsync();

        await bus.PublishAsync(new ProductCreated(product.Id, product.Name, product.Description, product.Price.Value, product.Category?.Name));

        return product.Id;
    }
}
