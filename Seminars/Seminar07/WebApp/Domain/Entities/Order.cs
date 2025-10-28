namespace Domain.Entities;

public class Order : BaseEntity
{
    public decimal Total { get; set; }

    public string FullName { get; set; } = default!;

    public string Address1 { get; set; } = default!;

    public string? Address2 { get; set; }

    public string City { get; set; } = default!;

    public string Zip { get; set; } = default!;

    public string Country { get; set; } = default!;

    public ICollection<OrderItem> Items { get; set; } = [];

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = default!;
}
