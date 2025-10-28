namespace Domain.Entities;

public class OrderItem : BaseEntity
{
    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;

    public int OrderId { get; set; }

    public Order Order { get; set; } = default!;
}
