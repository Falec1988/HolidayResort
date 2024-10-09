using HolidayResort.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HolidayResort.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Accommodation> Accommodations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accommodation>().HasData(
            new Accommodation
            {
                Id = 1,
                Name = "Soba",
                Description = "Moguće je rezervirati noćenje s doručkom, polupansion ili puni pansion.",
                Price = 60.00,
                SquareMeter = 40,
                Capacity = 2,
                ImageUrl = "https://placehold.co/600x400",
            },
            new Accommodation
            {
                Id = 2,
                Name = "Bungalov",
                Description = "Moguće je rezervirati noćenje s doručkom, polupansion ili puni pansion.",
                Price = 80.00,
                SquareMeter = 50,
                Capacity = 3,
                ImageUrl = "https://placehold.co/600x400",
            },
            new Accommodation
            {
                Id = 3,
                Name = "Apartman",
                Description = "Moguće je rezervirati noćenje s doručkom, polupansion ili puni pansion.",
                Price = 100.00,
                SquareMeter = 60,
                Capacity = 5,
                ImageUrl = "https://placehold.co/600x400",
            });
    }
}