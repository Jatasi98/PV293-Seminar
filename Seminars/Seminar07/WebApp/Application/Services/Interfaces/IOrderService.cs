using Application.DTOs;

namespace Application.Services.Interfaces;

public interface IOrderService
{
    Task<List<OrderDTO>?> GetOrders(CustomerDTO user);
    Task<OrderDTO?> GetOrder(CustomerDTO user, int orderId);
    Task<List<OrderDTO>> GetOrders();
    Task<OrderDTO?> FindOrderById(int id);
    Task UpdateOrder(OrderDTO dto);
}
