using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Services.Interface;
using HolidayResort.Application.Utility;
using HolidayResort.Web.ViewModels;

namespace HolidayResort.Application.Services.Implementation;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
    readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth - 1, 1);
    readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PieChartDto> GetBookingPieChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll(x => x.BookingDate >= DateTime.Now.AddDays(-30) &&
            (x.Status != SD.StatusPending || x.Status == SD.StatusCancelled));

        var customerWithOneBooking = totalBookings.GroupBy(x => x.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();

        int bookingsByNewCustomer = customerWithOneBooking.Count();

        int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

        PieChartDto PieChartDto = new()
        {
            Labels = new string[] { "Rezervacije novih korisnika", "Rezervacije starih korisnika" },
            Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
        };
        return PieChartDto;
    }

    public async Task<LineChartDto> GetMemberAndBookingLineChartData()
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

        LineChartDto LineChartDto = new()
        {
            Categories = categories,
            Series = chartDataList
        };
        return LineChartDto;
    }

    public async Task<RadialBarChartDto> GetRegisteredUserChartData()
    {
        var totalUsers = _unitOfWork.User.GetAll();

        var countByCurrentMonth = totalUsers.Count(x => x.CreatedAt >= currentMonthStartDate &&
        x.CreatedAt <= DateTime.Now);

        var countByPreviousMonth = totalUsers.Count(x => x.CreatedAt >= previousMonthStartDate &&
        x.CreatedAt <= currentMonthStartDate);

        return SD.GetRadialChartDataModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth);
    }

    public async Task<RadialBarChartDto> GetRevenueChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll(x => x.Status != SD.StatusPending
            || x.Status == SD.StatusCancelled);

        var totalRevenue = Convert.ToInt32(totalBookings.Sum(x => x.TotalCost));

        var countByCurrentMonth = totalBookings.Where(x => x.BookingDate >= currentMonthStartDate &&
        x.BookingDate <= DateTime.Now).Sum(x => x.TotalCost);

        var countByPreviousMonth = totalBookings.Where(x => x.BookingDate >= previousMonthStartDate &&
        x.BookingDate <= currentMonthStartDate).Sum(x => x.TotalCost);

        return SD.GetRadialChartDataModel(totalRevenue, countByCurrentMonth, countByPreviousMonth);
    }

    public async Task<RadialBarChartDto> GetTotalBookingRadialChartData()
    {
        var totalBookings = _unitOfWork.Booking.GetAll(x => x.Status != SD.StatusPending
            || x.Status == SD.StatusCancelled);

        var countByCurrentMonth = totalBookings.Count(x => x.BookingDate >= currentMonthStartDate &&
        x.BookingDate <= DateTime.Now);

        var countByPreviousMonth = totalBookings.Count(x => x.BookingDate >= previousMonthStartDate &&
        x.BookingDate <= currentMonthStartDate);

        return SD.GetRadialChartDataModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth);
    }
}