using Application.DTOs;

namespace Presentation.Models;

public class CartViewModel
{
    public List<CartItemDTO> Items { get; set; } = [];
    public int TotalQuantity => Items.Sum(i => i.Quantity);
    public decimal Subtotal => Items.Sum(i => i.TotalPrice);
    public decimal Shipping => Items.Count > 0 ? 5.00m : 0m;
    public decimal Total => Subtotal + Shipping;
}
