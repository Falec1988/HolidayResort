using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Services.Interface;

public interface IBookingService
{
    void CreateBooking(Booking booking);

    Booking GetBookingById(int bookingId);

    IEnumerable<Booking> GetAllBookings(string userId="", string? statusFilterList="");

    void UpdateStatus(int bookingId, string bookingStatus, int accommodationNo);

    void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId);

    public IEnumerable<int> GetCheckedInAccommodationNumbers(int accommodationId);
}