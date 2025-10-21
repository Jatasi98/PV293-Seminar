using System.ComponentModel.DataAnnotations;

namespace PV293WebApplication.Models;

public class CheckoutViewModel
{
    [Required] 
    public string FullName { get; set; } = "";
    [Required] 
    public string Address1 { get; set; } = "";
    public string? Address2 { get; set; }
    [Required]
    public string City { get; set; } = "";
    [Required] 
    public string Zip { get; set; } = "";
    [Required] 
    public string Country { get; set; } = "";
    public CartViewModel Cart { get; set; } = new();
}
