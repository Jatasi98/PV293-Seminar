using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Email or Username")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
