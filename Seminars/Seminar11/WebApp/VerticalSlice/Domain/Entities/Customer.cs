namespace VerticalSlice.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string? AppUserId { get; set; }

    public Cart? Cart { get; set; }
}
