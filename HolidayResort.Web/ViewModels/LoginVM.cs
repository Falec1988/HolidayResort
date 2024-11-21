using System.ComponentModel.DataAnnotations;

namespace HolidayResort.Web.ViewModels;

public class LoginVM
{
    [Required]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Lozinka")]
    public string Password { get; set; }

    [Display(Name = "Zapamti me")]
    public bool RememberMe { get; set; }

    public string? RedirectUrl { get; set; }
}