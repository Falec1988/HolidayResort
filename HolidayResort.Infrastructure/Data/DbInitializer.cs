using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Utility;
using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HolidayResort.Infrastructure.Data;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
    }

    public void Initialize()
    {
        try
        {
            if (_context.Database.GetPendingMigrations().Count() > 0)
            {
                _context.Database.Migrate();
            }
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@hvalecdavor.com",
                    Email = "admin@hvalecdavor.com",
                    Name = "Davor Hvalec",
                    NormalizedUserName = "ADMIN@HVALECDAVOR.COM",
                    NormalizedEmail = "ADMIN@HVALECDAVOR.COM",
                    PhoneNumber = "123123123",
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@hvalecdavor.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
        }
        catch(Exception)
        {
            throw;
        }
    }
}