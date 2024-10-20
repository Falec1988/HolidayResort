using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Utility;
using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using System.Globalization;
using System.Security.Claims;

namespace HolidayResort.Web.Controllers;

public class BookingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IWebHostEnvironment _webHostEnvironment;

    public BookingController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
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

        var accommodationNoList = _unitOfWork.AccommodationNumber.GetAll().ToList();

        var bookedAccommodation = _unitOfWork.Booking.GetAll(x => x.Status == SD.StatusApproved ||
        x.Status == SD.StatusCheckedIn).ToList();

        int roomAvailable = SD.AccommodationRoomsAvailableCount
                (accommodation.Id, accommodationNoList, booking.CheckInDate, booking.Nights, bookedAccommodation);

        if (roomAvailable == 0)
        {
            TempData["error"] = "Rasprodano!";

            return RedirectToAction(nameof(FinalizeBooking), new
            {
                accommodationId = booking.AccommodationId,
                checkInDate = booking.CheckInDate,
                nights = booking.Nights
            });
        };

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
                _unitOfWork.Booking.UpdateStatus(bookingFromDb.Id, SD.StatusApproved, 0);
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

    [HttpPost]
    [Authorize]
    public IActionResult GenerateInvoice(int id)
    {
        string basePath = _webHostEnvironment.WebRootPath;

        WordDocument document = new WordDocument();

        // Otvaranje dokumenta
        string dataPath = basePath + @"/exports/BookingDetails.docx";

        using FileStream fileStream = new(dataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        document.Open(fileStream, FormatType.Automatic);

        //Editiranje dokumenta
        Booking bookingFromDb = _unitOfWork.Booking.Get(x => x.Id == id,
            includeProperties: "User,Accommodation");

        TextSelection textSelection = document.Find("xx_customer_name", false, true);
        WTextRange textRange = textSelection.GetAsOneRange();
        textRange.Text = bookingFromDb.Name;

        textSelection = document.Find("xx_customer_phone", false, true);
        textRange = textSelection.GetAsOneRange();
        textRange.Text = bookingFromDb.Phone;

        textSelection = document.Find("xx_customer_email", false, true);
        textRange = textSelection.GetAsOneRange();
        textRange.Text = bookingFromDb.Email;

        textSelection = document.Find("XX_BOOKING_NUMBER", false, true);
        textRange = textSelection.GetAsOneRange();
        textRange.Text = "BROJ REZERVACIJE - " + bookingFromDb.Id;

        textSelection = document.Find("XX_BOOKING_DATE", false, true);
        textRange = textSelection.GetAsOneRange();
        textRange.Text = "DATUM REZERVACIJE - " + bookingFromDb.BookingDate.ToShortDateString();

        textSelection = document.Find("xx_payment_date", false, true);
        textRange = textSelection.GetAsOneRange();
        textRange.Text = bookingFromDb.PaymentDate.ToShortDateString();

        textSelection = document.Find("xx_checkin_date", false, true);
        textRange = textSelection.GetAsOneRange();
        textRange.Text = bookingFromDb.CheckInDate.ToShortDateString();

        textSelection = document.Find("xx_checkout_date", false, true);
        textRange = textSelection.GetAsOneRange();
        textRange.Text = bookingFromDb.CheckOutDate.ToShortDateString();

        textSelection = document.Find("xx_booking_total", false, true);
        textRange = textSelection.GetAsOneRange();
        textRange.Text = bookingFromDb.TotalCost.ToString("C", CultureInfo.CreateSpecificCulture("fr-FR"));

        WTable table = new(document);

        table.TableFormat.Borders.LineWidth = 1f;
        table.TableFormat.Borders.Color = Color.Black;
        table.TableFormat.Paddings.Top = 7f;
        table.TableFormat.Paddings.Bottom = 7f;
        table.TableFormat.Borders.Horizontal.LineWidth = 1f;

        table.ResetCells(2, 4);

        WTableRow row0 = table.Rows[0];

        row0.Cells[0].AddParagraph().AppendText("NOĆENJA");
        row0.Cells[0].Width = 80;
        row0.Cells[1].AddParagraph().AppendText("SMJEŠTAJ");
        row0.Cells[1].Width = 180;
        row0.Cells[2].AddParagraph().AppendText("CIJENA PO NOĆENJU");
        row0.Cells[3].AddParagraph().AppendText("UKUPNA CIJENA");
        row0.Cells[3].Width = 120;

        WTableRow row1 = table.Rows[1];

        row1.Cells[0].AddParagraph().AppendText(bookingFromDb.Nights.ToString());
        row1.Cells[0].Width = 80;
        row1.Cells[1].AddParagraph().AppendText(bookingFromDb.Accommodation.Name);
        row1.Cells[1].Width = 180;
        row1.Cells[2].AddParagraph().AppendText((bookingFromDb.TotalCost / bookingFromDb.Nights).ToString("C", CultureInfo.CreateSpecificCulture("fr-FR")));
        row1.Cells[3].AddParagraph().AppendText(bookingFromDb.TotalCost.ToString("C", CultureInfo.CreateSpecificCulture("fr-FR")));
        row1.Cells[3].Width = 120;

        WTableStyle tableStyle = document.AddTableStyle("CustomStyle") as WTableStyle;
        tableStyle.TableProperties.RowStripe = 1;
        tableStyle.TableProperties.ColumnStripe = 2;
        tableStyle.TableProperties.Paddings.Top = 2;
        tableStyle.TableProperties.Paddings.Bottom = 1;
        tableStyle.TableProperties.Paddings.Left = 5.4f;
        tableStyle.TableProperties.Paddings.Right = 5.4f;

        ConditionalFormattingStyle firstRowStyle = tableStyle.ConditionalFormattingStyles.Add(ConditionalFormattingType.FirstRow);
        firstRowStyle.CharacterFormat.Bold = true;
        firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
        firstRowStyle.CellProperties.BackColor = Color.Black;

        table.ApplyStyle("CustomStyle");

        TextBodyPart bodyPart = new(document);
        bodyPart.BodyItems.Add(table);

        document.Replace("<ADDTABLEHERE>", bodyPart, false, false);

        using DocIORenderer renderer = new();

        MemoryStream stream = new();
        document.Save(stream, FormatType.Docx);
        stream.Position = 0;

        return File(stream, "application/docx", "BookingDetails.docx");
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CheckIn(Booking booking)
    {
        _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCheckedIn, booking.AccommodationNo);
        _unitOfWork.Save();
        TempData["Success"] = "Status rezervacije promijenjen.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CheckOut(Booking booking)
    {
        _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCompleted, booking.AccommodationNo);
        _unitOfWork.Save();
        TempData["Success"] = "Rezervacija završena.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CancelBooking(Booking booking)
    {
        _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCancelled, 0);
        _unitOfWork.Save();
        TempData["Success"] = "Rezervacija otkazana.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
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