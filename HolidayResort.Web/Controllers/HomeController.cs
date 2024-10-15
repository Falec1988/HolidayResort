using HolidayResort.Application.Interfaces;
using HolidayResort.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index(HomeVM homeVM)
        {
            homeVM.AccommodationList = _unitOfWork.Accommodation.GetAll(includeProperties: "AccommodationEquipment");

            foreach (var accommodation in homeVM.AccommodationList)
            {
                if (accommodation.Id % 2 == 0)
                {
                    accommodation.IsAvailable = false;
                }
            }
            return View(homeVM);
        }

        public IActionResult GetAccommodationsByDate(int nights, DateOnly checkInDate)
        {
            var accommodationList = _unitOfWork.Accommodation.GetAll(includeProperties: "AccommodationEquipment").ToList();

            foreach (var accommodation in accommodationList)
            {
                if (accommodation.Id % 2 == 0)
                {
                    accommodation.IsAvailable = false;
                }
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
