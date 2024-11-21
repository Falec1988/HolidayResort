using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HolidayResort.Domain.Entities;

public class Equipment
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Naziv")]
    public required string Name { get; set; }

    [Display(Name = "Opis")]
    public string? Description { get; set; }

    [Display(Name = "Naziv smještaja")]
    [ForeignKey("Accommodation")]
    public int AccommodationId { get; set; }

    [ValidateNever]
    public Accommodation Accommodation { get; set; }
}