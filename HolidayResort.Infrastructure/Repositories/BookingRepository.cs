using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Utility;
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

    public void UpdateStatus(int bookingId, string bookingStatus, int accommodationNo=0)
    {
        var bookingFromDb = _context.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (bookingFromDb != null)
        {
            bookingFromDb.Status = bookingStatus;
            if (bookingStatus == SD.StatusCheckedIn)
            {
                bookingFromDb.AccommodationNo = accommodationNo;
                bookingFromDb.ActualCheckInDate = DateTime.Now;
            }
            if (bookingStatus == SD.StatusCompleted)
            {
                bookingFromDb.ActualCheckOutDate = DateTime.Now;
            }
        }
    }

    public void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId)
    {
        var bookingFromDb = _context.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (bookingFromDb != null)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                bookingFromDb.StripeSessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                bookingFromDb.StripePaymentIntentId = paymentIntentId;
                bookingFromDb.PaymentDate = DateTime.Now;
                bookingFromDb.IsPaymentSuccessful = true;
            }
        }
    }
}