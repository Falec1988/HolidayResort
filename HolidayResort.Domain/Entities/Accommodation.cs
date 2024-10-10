using System.ComponentModel.DataAnnotations;

namespace HolidayResort.Domain.Entities;

public class Accommodation
{
    public int Id { get; set; }
    [Display(Name="Naziv")]
    public required string Name { get; set; }
    [Display(Name = "Opis")]
    public string? Description { get; set; }
    [Display(Name = "Cijena po noćenju")]
    public double Price { get; set; }
    [Display(Name = "Kvadratura")]
    public int SquareMeter { get; set;}
    [Display(Name = "Kapacitet")]
    public int Capacity { get; set; }
    [Display(Name = "Slika")]
    public string? ImageUrl { get; set; }

    public DateTime CreatedDate { get; set;}

    public DateTime UpdatedDate { get; set; }
}