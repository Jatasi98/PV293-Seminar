namespace Module.Orders.Domain.Entities;

public class Order : BaseEntity
{
    public ICollection<OrderItem> Items { get; set; } = [];

    public int CustomerId { get; set; }
}
