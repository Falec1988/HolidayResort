using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HolidayResort.Domain.Entities;

public class AccommodationNumber
{
    [Display(Name = "Broj smještaja")]
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int AccommodationNo { get; set; }

    [Display(Name = "Naziv smještaja")]
    [ForeignKey("Accommodation")]
    public int AccommodationId { get; set; }

    [ValidateNever]
    public Accommodation Accommodation { get; set; }

    [Display(Name = "Detalji")]
    public string? SpecialDetails { get; set; }
}