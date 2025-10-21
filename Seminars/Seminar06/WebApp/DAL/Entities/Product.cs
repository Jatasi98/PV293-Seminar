using DAL.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class Product : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = default!;

    public decimal Price { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    public bool IsDeleted { get; set; } = false;

    [ForeignKey(nameof(Category))]
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
}
