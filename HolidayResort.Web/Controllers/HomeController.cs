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
