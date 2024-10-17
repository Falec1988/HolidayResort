using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HolidayResort.Web.Controllers;

public class BookingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult FinalizeBooking(int accommodationId, DateOnly checkInDate, int nights)
    {
        Booking booking = new()
        {
            AccommodationId = accommodationId,
            Accommodation = _unitOfWork.Accommodation.Get(x => x.Id == accommodationId, includeProperties: "AccommodationEquipment"),
            CheckInDate = checkInDate,
            Nights = nights,
            CheckOutDate = checkInDate.AddDays(nights)
        };

        booking.TotalCost = booking.Accommodation.Price * nights;

        return View(booking);
    }
}
