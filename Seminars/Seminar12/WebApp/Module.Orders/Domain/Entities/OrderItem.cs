namespace Module.Orders.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int ProductId { get; set; } = default!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    public int OrderId { get; set; }

    public Order Order { get; set; } = default!;
}
