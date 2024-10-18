using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Utility;
using HolidayResort.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace HolidayResort.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                AccommodationList = _unitOfWork.Accommodation.GetAll(includeProperties: "AccommodationEquipment"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };

            return View(homeVM);
        }

        [HttpPost]
        public IActionResult GetAccommodationsByDate(int nights, DateOnly checkInDate)
        {
            var accommodationList = _unitOfWork.Accommodation.GetAll(includeProperties: "AccommodationEquipment").ToList();

            var accommodationNoList = _unitOfWork.AccommodationNumber.GetAll().ToList();
            
            var bookedAccommodation = _unitOfWork.Booking.GetAll(x => x.Status == SD.StatusApproved ||
            x.Status == SD.StatusCheckedIn).ToList();


            foreach (var accommodation in accommodationList)
            {
                int roomAvailable = SD.AccommodationRoomsAvailableCount
                    (accommodation.Id, accommodationNoList, checkInDate, nights, bookedAccommodation);

                accommodation.IsAvailable = roomAvailable > 0 ? true : false;
            }
            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                AccommodationList = accommodationList,
                Nights = nights
            };

            return PartialView("_AccommodationList", homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
