using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using HolidayResort.Infrastructure.Data;

namespace HolidayResort.Infrastructure.Repositories;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationUserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

}