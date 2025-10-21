using BL.DTOs;
using System.Security.Claims;

namespace BL.Services;

public interface IOrderService
{
    Task<List<OrderDTO>?> GetOrders(ClaimsPrincipal user);
    Task<OrderDTO?> GetOrder(ClaimsPrincipal user, int orderId);
    Task<List<OrderDTO>> GetOrders();
    Task<OrderDTO?> FindOrderById(int id);
    Task UpdateOrder(OrderDTO dto);
}
