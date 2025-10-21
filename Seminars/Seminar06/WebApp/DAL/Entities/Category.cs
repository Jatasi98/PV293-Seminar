using DAL.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Category : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = default!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = [];
}