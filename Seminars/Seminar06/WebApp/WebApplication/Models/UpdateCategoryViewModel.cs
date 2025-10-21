using System.ComponentModel.DataAnnotations;

namespace PV293WebApplication.Models;

public class UpdateCategoryViewModel
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = default!;

    [StringLength(1000)]
    public string? Description { get; set; }
}
