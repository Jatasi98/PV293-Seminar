using DAL.Entities;

namespace PV293WebApplication.Models;

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public int TotalQuantity => Items.Sum(i => i.Quantity);
    public decimal Subtotal => Items.Sum(i => i.TotalPrice);
    public decimal Shipping => Items.Count > 0 ? 5.00m : 0m;
    public decimal Total => Subtotal + Shipping;
}
