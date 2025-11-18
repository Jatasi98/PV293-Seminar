namespace VerticalSlice.Domain.Entities;

public class CartItem : BaseEntity
{
    public int ProductId { get; set; } = default!;

    public Product Product { get; set; } = default!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    public int CartId { get; set; }

    public Cart Cart { get; set; } = default!;
}
