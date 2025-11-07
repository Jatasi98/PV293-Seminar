namespace VerticalSlice.Domain.Entities;

public class Cart : BaseEntity
{
    public ICollection<CartItem> Items { get; set; } = [];

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = default!;
}
