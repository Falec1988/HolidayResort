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

            return Json(GetRadialChartDataModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth));
        }

        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            var totalUsers = _unitOfWork.User.GetAll();

            var countByCurrentMonth = totalUsers.Count(x => x.CreatedAt >= currentMonthStartDate &&
            x.CreatedAt <= DateTime.Now);

            var countByPreviousMonth = totalUsers.Count(x => x.CreatedAt >= previousMonthStartDate &&
            x.CreatedAt <= currentMonthStartDate);

            return Json(GetRadialChartDataModel(totalUsers.Count(),countByCurrentMonth,countByPreviousMonth));
        }

        public async Task<IActionResult> GetRevenueChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(x => x.Status != SD.StatusPending
            || x.Status == SD.StatusCancelled);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(x => x.TotalCost));

            var countByCurrentMonth = totalBookings.Where(x => x.BookingDate >= currentMonthStartDate &&
            x.BookingDate <= DateTime.Now).Sum(x =>x.TotalCost);

            var countByPreviousMonth = totalBookings.Where(x => x.BookingDate >= previousMonthStartDate &&
            x.BookingDate <= currentMonthStartDate).Sum(x => x.TotalCost);

            return Json(GetRadialChartDataModel(totalRevenue, countByCurrentMonth, countByPreviousMonth));
        }

        public async Task<IActionResult> GetBookingPieChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(x => x.BookingDate >= DateTime.Now.AddDays(-30) &&
            (x.Status != SD.StatusPending || x.Status == SD.StatusCancelled));

            var customerWithOneBooking = totalBookings.GroupBy(x => x.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();

            int bookingsByNewCustomer = customerWithOneBooking.Count();

            int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

            PieChartVM pieChartVM = new()
            {
                Labels = new string[] { "Rezervacije novih korisnika", "Rezervacije starih korisnika" },
                Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
            };

            return Json(pieChartVM);
        }

        public async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            var bookingData = _unitOfWork.Booking.GetAll(x => x.BookingDate >= DateTime.Now.AddDays(-30) &&
            x.BookingDate.Date <= DateTime.Now)
                .GroupBy(x => x.BookingDate.Date)
                .Select(x => new
                {
                    DateTime = x.Key,
                    NewBookingCount = x.Count(),
                });

            var customerData = _unitOfWork.User.GetAll(x => x.CreatedAt >= DateTime.Now.AddDays(-30) &&
            x.CreatedAt.Date <= DateTime.Now)
                .GroupBy(x => x.CreatedAt.Date)
                .Select(x => new
                {
                    DateTime = x.Key,
                    NewCustomerCount = x.Count(),
                });

            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.DateTime, customer => customer.DateTime,
                (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                });

            var rightJoin = customerData.GroupJoin(bookingData, customer => customer.DateTime, booking => booking.DateTime,
                (customer, booking) => new
                {
                    customer.DateTime,
                    NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),
                    customer.NewCustomerCount
                });

            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

            var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();

            var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();

            var categories = mergedData.Select(x => x.DateTime.ToString("dd/MM/yyyy")).ToArray();

            List<ChartData> chartDataList = new()
            {
                new ChartData {
                    Name = "Nove rezervacije",
                    Data = newBookingData
                },
                new ChartData {
                    Name = "Novi korisnici",
                    Data = newCustomerData
                },
            };

            LineChartVM lineChartVM = new()
            {
                Categories = categories,
                Series = chartDataList
            };

            return Json(lineChartVM);
        }

        private static RadialBarChartVM GetRadialChartDataModel(int totalCount, double currentMonthCount, double prevMonthCount)
        {
            RadialBarChartVM radialBarChartVM = new();

            int increaseDecreaseRatio = 100;

            if (prevMonthCount != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((currentMonthCount - prevMonthCount) / prevMonthCount * 100);
            }

            radialBarChartVM.TotalCount = totalCount;

            radialBarChartVM.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);

            radialBarChartVM.HasRatioIncreased = currentMonthCount > prevMonthCount;

            radialBarChartVM.Series = new int[] { increaseDecreaseRatio };

            return radialBarChartVM;
        }
    }
}
