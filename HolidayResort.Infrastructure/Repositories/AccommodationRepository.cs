using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using HolidayResort.Infrastructure.Data;

namespace HolidayResort.Infrastructure.Repositories;

public class AccommodationRepository : Repository<Accommodation>, IAccommodationRepository
{
    private readonly ApplicationDbContext _context;

    public AccommodationRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public void Update(Accommodation entity)
    {
        _context.Accommodations.Update(entity);
    }
}