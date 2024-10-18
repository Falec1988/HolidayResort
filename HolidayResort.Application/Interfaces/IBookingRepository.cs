using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    void Update(Booking entity);

    void UpdateStatus(int bookingId, string bookingStatus, int accommodationNo);

    void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId);
}