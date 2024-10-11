using HolidayResort.Application.Interfaces;
using HolidayResort.Infrastructure.Data;

namespace HolidayResort.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IAccommodationRepository Accommodation { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Accommodation = new AccommodationRepository(_context);
    }
}