using DAL.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class CartItem : BaseEntity
{
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = default!;

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    [NotMapped]
    public decimal TotalPrice => UnitPrice * Quantity;

    [ForeignKey(nameof(Cart))]
    public int CartId { get; set; }

    [Required]
    public virtual Cart Cart { get; set; } = default!;
}
