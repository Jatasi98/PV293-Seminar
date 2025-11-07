using VerticalSlice.DTOs;

namespace VerticalSlice.Features.Cart.Models;

public class CartModel
{
    // No AccountController available — using a proxy customerId instead.
    public int CustomerId { get; set; } = 1;
    public List<CartItemDTO> Items { get; set; } = [];
    public int TotalQuantity => Items.Sum(i => i.Quantity);
    public decimal Subtotal => Items.Sum(i => i.TotalPrice);
    public decimal Shipping => Items.Count > 0 ? 5.00m : 0m;
    public decimal Total => Subtotal + Shipping;
}
