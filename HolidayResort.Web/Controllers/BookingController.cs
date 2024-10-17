using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Utility;
using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HolidayResort.Web.Controllers;

public class BookingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [Authorize]
    public IActionResult FinalizeBooking(int accommodationId, DateOnly checkInDate, int nights)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ApplicationUser user = _unitOfWork.User.Get(x => x.Id == userId);

        Booking booking = new()
        {
            AccommodationId = accommodationId,
            Accommodation = _unitOfWork.Accommodation.Get(x => x.Id == accommodationId, includeProperties: "AccommodationEquipment"),
            CheckInDate = checkInDate,
            Nights = nights,
            CheckOutDate = checkInDate.AddDays(nights),
            UserId = userId,
            Phone = user.PhoneNumber,
            Email = user.Email,
            Name = user.Name
        };

        booking.TotalCost = booking.Accommodation.Price * nights;

        return View(booking);
    }

    [Authorize]
    [HttpPost]
    public IActionResult FinalizeBooking(Booking booking)
    {
        var accommodation = _unitOfWork.Accommodation.Get(x => x.Id == booking.AccommodationId);
        booking.TotalCost = accommodation.Price * booking.Nights;

        booking.Status = SD.StatusPending;
        booking.BookingDate = DateTime.Now;

        _unitOfWork.Booking.Add(booking);
        _unitOfWork.Save();

        return RedirectToAction(nameof(BookingConfirmation), new { bookingId = booking.Id });
    }

    [Authorize]
    public IActionResult BookingConfirmation(int bookingId)
    {
        return View(bookingId);
    }
}