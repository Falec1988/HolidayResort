using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Utility;
using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
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
    public IActionResult Index()
    {
        return View();
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

        var domain = Request.Scheme + "://" + Request.Host.Value + "/";

        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
            CancelUrl = domain + $"booking/FinalizeBooking?accommodationId={booking.AccommodationId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}"
        };

        options.LineItems.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (long)(booking.TotalCost * 100),
                Currency = "eur",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = accommodation.Name
                },
            },
            Quantity = 1
        });

        var service = new SessionService();
        Session session = service.Create(options);

        _unitOfWork.Booking.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();

        Response.Headers.Add("Location", session.Url);

        return new StatusCodeResult(303);
    }

    [Authorize]
    public IActionResult BookingConfirmation(int bookingId)
    {
        Booking bookingFromDb = _unitOfWork.Booking.Get(x => x.Id == bookingId, includeProperties: "User,Accommodation");

        if (bookingFromDb.Status == SD.StatusPending)
        {
            var service = new SessionService();
            Session session = service.Get(bookingFromDb.StripeSessionId);

            if (session.PaymentStatus == "paid")
            {
                _unitOfWork.Booking.UpdateStatus(bookingFromDb.Id, SD.StatusApproved,0);
                _unitOfWork.Booking.UpdateStripePaymentID(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
            }
        }

        return View(bookingId);
    }

    [Authorize]
    public IActionResult BookingDetails(int bookingId)
    {
        Booking bookingFromDb = _unitOfWork.Booking.Get(x => x.Id == bookingId,
            includeProperties: "User,Accommodation");

        if (bookingFromDb.AccommodationNo == 0 && bookingFromDb.Status == SD.StatusApproved)
        {
            var availableAccommodationNumber = AssignAvailableAccommodationNoByAccommodation(bookingFromDb.AccommodationId);

            bookingFromDb.AccommodationNumber = _unitOfWork.AccommodationNumber.GetAll(x => x.AccommodationId == bookingFromDb.AccommodationId
            && availableAccommodationNumber.Any(a => a == x.AccommodationNo)).ToList();
        }

        return View(bookingFromDb);
    }

    private List<int> AssignAvailableAccommodationNoByAccommodation(int accommodationId)
    {
        List<int> availableAccommodationNumbers = new();

        var accommodationNumbers = _unitOfWork.AccommodationNumber.GetAll(x => x.AccommodationId == accommodationId);

        var checkedInAccommodation = _unitOfWork.Booking.GetAll(x => x.AccommodationId == accommodationId && x.Status == SD.StatusCheckedIn)
            .Select(x => x.AccommodationNo);

        foreach (var accommodationNo in accommodationNumbers)
        {
            if (!checkedInAccommodation.Contains(accommodationNo.AccommodationNo))
            {
                availableAccommodationNumbers.Add(accommodationNo.AccommodationNo);
            }
        }

        return availableAccommodationNumbers;
    }

    #region API Calls
    [HttpGet]
    [Authorize]
    public IActionResult GetAll(string status)
    {
        IEnumerable<Booking> objBookings;
        if (User.IsInRole(SD.Role_Admin))
        {
            objBookings = _unitOfWork.Booking.GetAll(includeProperties: "User,Accommodation");
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            objBookings = _unitOfWork.Booking
                .GetAll(x => x.UserId == userId, includeProperties: "User,Accommodation");
        }
        if (!string.IsNullOrEmpty(status))
        {
            objBookings = objBookings.Where(x => x.Status.ToLower().Equals(status.ToLower()));
        }

        return Json(new { data = objBookings });
    }
    #endregion
}