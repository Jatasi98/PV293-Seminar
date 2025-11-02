namespace VerticalSlice.Domain.Entities;

public class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedOnUTC { get; set; } = DateTime.UtcNow;
}
