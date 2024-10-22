using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using HolidayResort.Infrastructure.Data;

namespace HolidayResort.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Booking entity)
    {
        _context.Bookings.Update(entity);
    }
}