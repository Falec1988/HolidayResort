using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HolidayResort.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Accommodation> Accommodations { get; set; }

    public DbSet<AccommodationNumber> AccommodationNumbers { get; set; }

    public DbSet<Equipment> Equipments { get; set; }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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

        modelBuilder.Entity<AccommodationNumber>().HasData(
            new AccommodationNumber
            {
                AccommodationNo = 101,
                AccommodationId = 1,
            },
            new AccommodationNumber
            {
                AccommodationNo = 102,
                AccommodationId = 1,
            },
            new AccommodationNumber
            {
                AccommodationNo = 103,
                AccommodationId = 1,
            },
            new AccommodationNumber
            {
                AccommodationNo = 104,
                AccommodationId = 1,
            },
            new AccommodationNumber
            {
                AccommodationNo = 201,
                AccommodationId = 2,
            },
            new AccommodationNumber
            {
                AccommodationNo = 202,
                AccommodationId = 2,
            },
            new AccommodationNumber
            {
                AccommodationNo = 203,
                AccommodationId = 2,
            },
            new AccommodationNumber
            {
                AccommodationNo = 301,
                AccommodationId = 3,
            },
            new AccommodationNumber
            {
                AccommodationNo = 302,
                AccommodationId = 3,
            });

        modelBuilder.Entity<Equipment>().HasData(
            new Equipment
            {
                Id = 1,
                AccommodationId = 1,
                Name = "Bračni krevet"
            },
            new Equipment
            {
                Id = 2,
                AccommodationId = 1,
                Name = "Sef"
            },
            new Equipment
            {
                Id = 3,
                AccommodationId = 1,
                Name = "Fen"
            },

            new Equipment
            {
                Id = 4,
                AccommodationId = 2,
                Name = "Bračni krevet i sofa na razvlačenje"
            },
            new Equipment
            {
                Id = 5,
                AccommodationId = 2,
                Name = "Minibar"
            },
            new Equipment
            {
                Id = 6,
                AccommodationId = 2,
                Name = "Klima uređaj"
            },

            new Equipment
            {
                Id = 7,
                AccommodationId = 3,
                Name = "Bračni krevet, sofa i kauč na razvlačenje"
            },
            new Equipment
            {
                Id = 8,
                AccommodationId = 3,
                Name = "Perilica za pranje suđa"
            },
            new Equipment
            {
                Id = 9,
                AccommodationId = 3,
                Name = "Kuhinja"
            });
    }
}