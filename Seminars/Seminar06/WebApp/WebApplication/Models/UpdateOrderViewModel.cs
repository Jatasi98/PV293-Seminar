using System.ComponentModel.DataAnnotations;

namespace PV293WebApplication.Models;

public class UpdateOrderViewModel
{
    public int Id { get; set; }

    public decimal Total { get; set; }

    [Required, StringLength(200)]
    public string FullName { get; set; } = default!;

    [Required, StringLength(200)]
    public string Address1 { get; set; } = default!;

    [StringLength(200)]
    public string? Address2 { get; set; }

    [Required, StringLength(100)]
    public string City { get; set; } = default!;

    [Required, StringLength(20)]
    public string Zip { get; set; } = default!;

    [Required, StringLength(100)]
    public string Country { get; set; } = default!;

    public int CustomerId { get; set; }
}
