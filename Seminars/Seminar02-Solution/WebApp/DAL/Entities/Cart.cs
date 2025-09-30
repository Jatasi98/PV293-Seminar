using DAL.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class Cart : BaseEntity
{
    public virtual ICollection<CartItem> Items { get; set; } = [];

    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }

    [Required]
    public virtual Customer Customer { get; set; } = default!;
}
