using DAL.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class Order : BaseEntity
{
    public decimal Total { get; set; }

    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = default!;

    [Required]
    [StringLength(200)]
    public string Address1 { get; set; } = default!;

    [StringLength(200)]
    public string? Address2 { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = default!;

    [Required]
    [StringLength(20)]
    public string Zip { get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = default!;

    public virtual ICollection<OrderItem> Items { get; set; } = [];

    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = default!;
}
