using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BL.Services;

public class OrderService : IOrderService
{
    private readonly WebAppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public OrderService(WebAppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<List<Order>?> GetOrders(ClaimsPrincipal user)
    {
        var appUserId = _userManager.GetUserId(user);

        var customerId = await _db.Customers
            .Where(c => c.AppUserId == appUserId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        if (customerId == 0)
        {
            return null;
        }

        var orders = await _db.Orders
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedOnUTC)
            .AsNoTracking()
            .ToListAsync();

        return orders;
    }

    public async Task<Order?> GetOrder(ClaimsPrincipal user, int orderId)
    {
        var appUserId = _userManager.GetUserId(user);

        var customerId = await _db.Customers
            .Where(c => c.AppUserId == appUserId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        var order = await _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId && (customerId == 0 || o.CustomerId == customerId));

        return order;
    }
}
