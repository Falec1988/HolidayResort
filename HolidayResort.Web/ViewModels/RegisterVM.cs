using System.ComponentModel.DataAnnotations;

namespace HolidayResort.Web.ViewModels;

public class RegisterVM
{
    [Required]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Lozinka")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    [Display(Name = "Ponovi lozinku")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string Name { get; set; }

    [Display(Name="Broj telefona")]
    public string? PhoneNumber { get; set; }

    public string? RedirectUrl { get; set; }
}