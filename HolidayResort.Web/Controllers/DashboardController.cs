using HolidayResort.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HolidayResort.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashbordService;

        public DashboardController(IDashboardService dashbordService)
        {
            _dashbordService = dashbordService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            return Json(await _dashbordService.GetTotalBookingRadialChartData());
        }

        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            return Json(await _dashbordService.GetRegisteredUserChartData());
        }

        public async Task<IActionResult> GetRevenueChartData()
        {
            return Json(await _dashbordService.GetRevenueChartData());
        }

        public async Task<IActionResult> GetBookingPieChartData()
        {
            return Json(await _dashbordService.GetBookingPieChartData());
        }

        public async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            return Json(await _dashbordService.GetMemberAndBookingLineChartData());
        } 
    }
}
