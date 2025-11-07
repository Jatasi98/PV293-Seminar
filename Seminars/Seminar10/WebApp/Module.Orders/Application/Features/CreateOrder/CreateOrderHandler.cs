using MediatR;
using Module.Orders.Domain.Entities;
using Module.Orders.Infrastructure;

namespace Module.Orders.Application.Features.CreateOrder;

public class CreateOrderHandler(OrderDbContext orderDbContext) : IRequestHandler<CreateOrderCommand, int>
{
    private readonly OrderDbContext orderDbContext = orderDbContext;

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var newOrder = new Order()
        {
            CustomerId = request.Customer.Id,
            Items = [.. request.Items.Select(x => new OrderItem() { ProductId = x.ProductId, Quantity = x.Quantity, UnitPrice = x.UnitPrice })],
        };

        orderDbContext.Add(newOrder);

        await orderDbContext.SaveChangesAsync();

        return newOrder.Id;
    }
}
