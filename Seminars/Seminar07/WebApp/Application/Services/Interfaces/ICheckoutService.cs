using Application.DTOs;

namespace Application.Services.Interfaces;

public interface ICheckoutService
{
    Task<int?> PlaceOrder(PlaceOrderDTO orderToCreate, CustomerDTO user);

    Task<OrderDTO> Confirmation(int id);
}
