using DAL.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Customer : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = default!;

    [Required]
    [EmailAddress]
    [StringLength(400)]
    public string Email { get; set; } = default!;

    public string? AppUserId { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = [];
}
