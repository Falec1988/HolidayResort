using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Services.Interface;
using HolidayResort.Application.Utility;
using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Services.Implementation;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void CreateBooking(Booking booking)
    {
        _unitOfWork.Booking.Add(booking);
        _unitOfWork.Save();
    }

    public IEnumerable<Booking> GetAllBookings(string userId = "", string? statusFilterList = "")
    {
        IEnumerable<string> statusList = statusFilterList.ToLower().Split(",");

        if (!string.IsNullOrEmpty(statusFilterList) && !string.IsNullOrEmpty(userId))
        {
            return _unitOfWork.Booking.GetAll(x => statusList.Contains(x.Status.ToLower()) &&
            x.UserId == userId, includeProperties: "User,Accommodation");
        }
        else
        {
            if (!string.IsNullOrEmpty(statusFilterList))
            {
                return _unitOfWork.Booking.GetAll(x => statusList.Contains(x.Status.ToLower()), includeProperties: "User,Accommodation");
            }
            if (!string.IsNullOrEmpty(userId))
            {
                return _unitOfWork.Booking.GetAll(x => x.UserId == userId, includeProperties: "User,Accommodation");
            }
        }
        return _unitOfWork.Booking.GetAll(includeProperties: "User,Accommodation");
    }

    public Booking GetBookingById(int bookingId)
    {
        return _unitOfWork.Booking.Get(x => x.Id == bookingId, includeProperties: "User,Accommodation");
    }

    public IEnumerable<int> GetCheckedInAccommodationNumbers(int accommodationId)
    {
        return _unitOfWork.Booking.GetAll(x => x.AccommodationId == accommodationId && x.Status == SD.StatusCheckedIn)
            .Select(x => x.AccommodationNo);
    }

    public void UpdateStatus(int bookingId, string bookingStatus, int accommodationNo = 0)
    {
        var bookingFromDb = _unitOfWork.Booking.Get(x => x.Id == bookingId, tracked: true);
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
        _unitOfWork.Save();
    }

    public void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId)
    {
        var bookingFromDb = _unitOfWork.Booking.Get(x => x.Id == bookingId, tracked: true);
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
        _unitOfWork.Save();
    }
}