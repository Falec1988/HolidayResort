using HolidayResort.Application.Interfaces;
using HolidayResort.Infrastructure.Data;

namespace HolidayResort.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IAccommodationRepository Accommodation { get; private set; }

    public IAccommodationNumberRepository AccommodationNumber { get; private set; }

    public IApplicationUserRepository User { get; private set; }

    public IEquipmentRepository Equipment { get; private set; }

    public IBookingRepository Booking { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Accommodation = new AccommodationRepository(_context);
        AccommodationNumber = new AccommodationNumberRepository(_context);
        Equipment = new EquipmentRepository(_context);
        Booking = new BookingRepository(_context);
        User = new ApplicationUserRepository(_context);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}