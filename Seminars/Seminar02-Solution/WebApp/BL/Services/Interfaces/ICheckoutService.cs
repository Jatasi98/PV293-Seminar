using BL.DTOs;
using DAL.Entities;
using System.Security.Claims;

namespace BL.Services;

public interface ICheckoutService
{
    Task<int?> PlaceOrder(PlaceOrderDTO orderToCreate, ClaimsPrincipal user);
    Task<Order> Confirmation(int id);
}
