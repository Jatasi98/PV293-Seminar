namespace WebApplication1.Entities;

public class Order : BaseEntity
{
    public decimal Total { get; set; }
    public string FullName { get; set; } = "";
    public string Address1 { get; set; } = "";
    public string? Address2 { get; set; }
    public string City { get; set; } = "";
    public string Zip { get; set; } = "";
    public string Country { get; set; } = "";

    public List<OrderItem> Items { get; set; } = new();

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
}
