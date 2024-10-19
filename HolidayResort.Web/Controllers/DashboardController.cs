using Microsoft.AspNetCore.Mvc;

namespace HolidayResort.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
