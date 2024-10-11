using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using HolidayResort.Infrastructure.Data;

namespace HolidayResort.Infrastructure.Repositories;

public class AccommodationNumberRepository : Repository<AccommodationNumber>, IAccommodationNumberRepository
{
    private readonly ApplicationDbContext _context;

    public AccommodationNumberRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(AccommodationNumber entity)
    {
        _context.AccommodationNumbers.Update(entity);
    }
}