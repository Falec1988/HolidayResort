using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Utility;
using HolidayResort.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HolidayResort.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;

        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth - 1, 1);

        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(x => x.Status != SD.StatusPending
            || x.Status == SD.StatusCancelled);

            var countByCurrentMonth = totalBookings.Count(x => x.BookingDate >= currentMonthStartDate &&
            x.BookingDate <= DateTime.Now);

            var countByPreviousMonth = totalBookings.Count(x => x.BookingDate >= previousMonthStartDate &&
            x.BookingDate <= currentMonthStartDate);

            RadialBarChartVM radialBarChartVM = new();

            int increaseDecreaseRatio = 100;

            if (countByPreviousMonth != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((countByCurrentMonth - countByPreviousMonth) / countByPreviousMonth * 100);
            }

            radialBarChartVM.TotalCount = totalBookings.Count();

            radialBarChartVM.CountInCurrentMonth = countByCurrentMonth;

            radialBarChartVM.HasRatioIncreased = currentMonthStartDate > previousMonthStartDate;

            radialBarChartVM.Series = new int[] { increaseDecreaseRatio };

            return Json(radialBarChartVM);
        }
    }
}
