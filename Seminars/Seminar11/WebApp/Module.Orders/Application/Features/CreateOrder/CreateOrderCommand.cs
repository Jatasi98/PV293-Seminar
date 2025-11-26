using MediatR;
using Module.Orders.Application.DTOs;

namespace Module.Orders.Application.Features.CreateOrder;

public sealed record CreateOrderCommand(OrderCustomerDTO Customer, IEnumerable<OrderItemDTO> Items) : IRequest<int>;
