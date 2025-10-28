using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class CreateProductViewModel
{
    [Required, StringLength(200)]
    public string Name { get; set; } = default!;

    public decimal Price { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public int? CategoryId { get; set; }
}
