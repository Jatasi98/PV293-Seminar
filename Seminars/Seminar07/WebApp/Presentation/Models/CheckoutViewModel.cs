using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class CheckoutViewModel
{
    [Required] 
    public string FullName { get; set; } = string.Empty;
    [Required] 
    public string Address1 { get; set; } = string.Empty;
    public string? Address2 { get; set; }
    [Required]
    public string City { get; set; } = string.Empty;
    [Required] 
    public string Zip { get; set; } = string.Empty;
    [Required] 
    public string Country { get; set; } = string.Empty;
    public CartViewModel Cart { get; set; } = new();
}
