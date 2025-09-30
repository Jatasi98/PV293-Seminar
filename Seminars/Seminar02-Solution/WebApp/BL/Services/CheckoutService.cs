using BL.DTOs;
using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BL.Services;

public class CheckoutService : ICheckoutService
{
    private readonly WebAppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public CheckoutService(WebAppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<int?> PlaceOrder(PlaceOrderDTO orderToCreate, ClaimsPrincipal user)
    {
        var appUserId = _userManager.GetUserId(user);

        var customerId = await _db.Customers
            .Where(x => x.AppUserId == appUserId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (customerId == 0)
        {
            return null;
        }

        var cartEntity = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cartEntity == null || !cartEntity.Items.Any())
        {
            return null;
        }

        var productIds = cartEntity.Items.Select(i => i.ProductId).ToList();
        var products = await _db.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var item in cartEntity.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
            {
                return null;
            }

            item.UnitPrice = product.Price;
        }

        var subtotal = cartEntity.Items.Sum(i => i.UnitPrice * i.Quantity);
        var shipping = cartEntity.Items.Count > 0 ? 5.00m : 0m;
        var total = subtotal + shipping;

        var order = new Order
        {
            CustomerId = customerId,
            CreatedOnUTC = DateTime.UtcNow,
            Total = total,
            FullName = orderToCreate.FullName,
            Address1 = orderToCreate.Address1,
            Address2 = orderToCreate.Address2,
            City = orderToCreate.City,
            Zip = orderToCreate.Zip,
            Country = orderToCreate.Country,
            Items = [.. cartEntity.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            })]
        };

        _db.Orders.Add(order);
        _db.Carts.Remove(cartEntity);

        await _db.SaveChangesAsync();

        return order.Id;
    }

    public async Task<Order?> Confirmation(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        return order;
    }
}
