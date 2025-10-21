using System.ComponentModel.DataAnnotations;

namespace PV293WebApplication.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Email or Username")]
    public string Email { get; set; } = "";

    [Required]
    [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
