using Microsoft.EntityFrameworkCore;

namespace HolidayResort.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    protected ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}