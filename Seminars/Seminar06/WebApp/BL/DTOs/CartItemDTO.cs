using System.ComponentModel.DataAnnotations.Schema;

namespace BL.DTOs;

public class CartItemDTO
{
    public string ProductName { get; set; } = default!;
    public int ProductId { get; set; }

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    [NotMapped]
    public decimal TotalPrice => UnitPrice * Quantity;
}
