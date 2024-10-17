using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HolidayResort.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    [Display(Name = "Ime i prezime")]
    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }
}