using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public int TotalQuantity => Items.Sum(i => i.Quantity);
        public decimal Subtotal => Items.Sum(i => i.LineTotal);
        public decimal Shipping => Items.Count > 0 ? 5.00m : 0m;
        public decimal Total => Subtotal + Shipping;
    }
}
