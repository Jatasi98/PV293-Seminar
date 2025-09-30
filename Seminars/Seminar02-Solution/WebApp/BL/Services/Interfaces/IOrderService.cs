using DAL.Entities;
using System.Security.Claims;

namespace BL.Services;

public interface IOrderService
{
    Task<List<Order>?> GetOrders(ClaimsPrincipal user);

    Task<Order?> GetOrder(ClaimsPrincipal user, int orderId);
}
