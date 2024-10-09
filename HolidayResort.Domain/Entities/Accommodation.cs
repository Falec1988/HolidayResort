namespace HolidayResort.Domain.Entities;

public class Accommodation
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public double Price { get; set; }

    public int SquareMeter { get; set;}

    public int Capacity { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedDate { get; set;}

    public DateTime UpdatedDate { get; set; }
}