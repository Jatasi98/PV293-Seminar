using BL.DTOs;
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

    public async Task<List<OrderDTO>?> GetOrders(ClaimsPrincipal user)
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
            .Select(o => new OrderDTO
            {
                Id = o.Id,
                Total = o.Total,
                FullName = o.FullName,
                Address1 = o.Address1,
                Address2 = o.Address2,
                City = o.City,
                Zip = o.Zip,
                Country = o.Country,
                CustomerId = o.CustomerId
            })
            .ToListAsync();

        return orders;
    }

    public async Task<OrderDTO?> GetOrder(ClaimsPrincipal user, int orderId)
    {
        var appUserId = _userManager.GetUserId(user);

        var customerId = await _db.Customers
            .Where(c => c.AppUserId == appUserId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        var order = await _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .Select(o => new OrderDTO
            {
                Id = o.Id,
                Total = o.Total,
                FullName = o.FullName,
                Address1 = o.Address1,
                Address2 = o.Address2,
                City = o.City,
                Zip = o.Zip,
                Country = o.Country,
                CustomerId = o.CustomerId
            })
            .FirstOrDefaultAsync(o => o.Id == orderId && (customerId == 0 || o.CustomerId == customerId));

        return order;
    }

    public async Task<List<OrderDTO>> GetOrders()
    {
        var orders = await _db.Orders
                .AsNoTracking()
                .Select(o => new OrderDTO
                {
                    Id = o.Id,
                    Total = o.Total,
                    FullName = o.FullName,
                    Address1 = o.Address1,
                    Address2 = o.Address2,
                    City = o.City,
                    Zip = o.Zip,
                    Country = o.Country,
                    CustomerId = o.CustomerId
                })
                .ToListAsync();

        return orders;
    }

    public async Task<OrderDTO?> FindOrderById(int id)
    {
        var order = await _db.Orders
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new OrderDTO
                {
                    Id = x.Id,
                    Total = x.Total,
                    FullName = x.FullName,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    City = x.City,
                    Zip = x.Zip,
                    Country = x.Country,
                    CustomerId = x.CustomerId
                })
                .FirstOrDefaultAsync();

        if (order == null)
        {
            return null;
        }

        return order;
    }

    public async Task UpdateOrder(OrderDTO dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var entity = await _db.Orders.FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        entity.Total = dto.Total;
        entity.FullName = dto.FullName;
        entity.Address1 = dto.Address1;
        entity.Address2 = dto.Address2;
        entity.City = dto.City;
        entity.Zip = dto.Zip;
        entity.Country = dto.Country;
        entity.CustomerId = dto.CustomerId;

        await _db.SaveChangesAsync();
    }
}
