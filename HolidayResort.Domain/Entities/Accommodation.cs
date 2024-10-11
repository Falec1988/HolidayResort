using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HolidayResort.Domain.Entities;

public class Accommodation
{
    public int Id { get; set; }

    [MaxLength(50)]
    [Display(Name = "Naziv")]
    public required string Name { get; set; }

    [Display(Name = "Opis")]
    public string? Description { get; set; }

    [Range(10,10000, ErrorMessage = "Minimalna vrijednost 10, a maksimalna vrijednost 10.000!")]
    [Display(Name = "Cijena po noćenju")]
    public double Price { get; set; }

    [Display(Name = "Kvadratura")]
    public int SquareMeter { get; set;}

    [Range(1,10, ErrorMessage = "Maksimalna vrijednost 10!")]
    [Display(Name = "Kapacitet")]
    public int Capacity { get; set; }

    [Display(Name = "Slika")]
    public string? ImageUrl { get; set; }

    [NotMapped]
    public IFormFile? Image { get; set; }

    public DateTime CreatedDate { get; set;}

    public DateTime UpdatedDate { get; set; }
}