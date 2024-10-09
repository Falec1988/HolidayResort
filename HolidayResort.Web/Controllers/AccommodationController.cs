using HolidayResort.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace HolidayResort.Web.Controllers;

public class AccommodationController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccommodationController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var accommodations = _context.Accommodations.ToList();
        return View(accommodations);
    }
}