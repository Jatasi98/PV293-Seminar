using DAL.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class OrderItem : BaseEntity
{
    [ForeignKey(nameof(Product))]
    public int? ProductId { get; set; }
    public virtual Product? Product { get; set; }

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    [NotMapped]
    public decimal LineTotal => UnitPrice * Quantity;

    [ForeignKey(nameof(Order))]
    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = default!;
}
