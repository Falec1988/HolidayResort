using HolidayResort.Domain.Entities;
using HolidayResort.Infrastructure.Data;
using HolidayResort.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HolidayResort.Web.Controllers;

public class AccommodationNumberController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccommodationNumberController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var accommodationNumbers = _context.AccommodationNumbers.Include(x => x.Accommodation).ToList();
        return View(accommodationNumbers);
    }

    public IActionResult Create()
    {
        AccommodationNumberVM accommodationNumberVM = new()
        {
            AccommodationList = _context.Accommodations.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
        };
        return View(accommodationNumberVM);
    }

    [HttpPost]
    public IActionResult Create(AccommodationNumberVM obj)
    {
        bool accommodationNoExists = _context.AccommodationNumbers.Any(x => x.AccommodationNo == obj.AccommodationNumber.AccommodationNo);



        if (ModelState.IsValid && !accommodationNoExists)
        {
            _context.AccommodationNumbers.Add(obj.AccommodationNumber);
            _context.SaveChanges();
            TempData["success"] = "Broj smještaja je uspješno kreiran.";
            return RedirectToAction("Index");
        }

        if (accommodationNoExists)
        {
            TempData["error"] = "Broj smještaja već postoji.";
        };

        obj.AccommodationList = _context.Accommodations.ToList().Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });

        return View(obj);
    }

    public IActionResult Update(int accommodationNumberId)
    {
        AccommodationNumberVM accommodationNumberVM = new()
        {
            AccommodationList = _context.Accommodations.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
            AccommodationNumber = _context.AccommodationNumbers.FirstOrDefault(x => x.AccommodationNo == accommodationNumberId)
        };

        if (accommodationNumberVM.AccommodationNumber is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(accommodationNumberVM);
    }

    [HttpPost]
    public IActionResult Update(Accommodation obj)
    {
        if (ModelState.IsValid && obj.Id > 0)
        {
            _context.Accommodations.Update(obj);
            _context.SaveChanges();
            TempData["success"] = "Broj smještaja je uspješno uređen.";
            return RedirectToAction("Index");
        }
        return View();
    }

    public IActionResult Delete(int accommodationId)
    {
        Accommodation? obj = _context.Accommodations.FirstOrDefault(x => x.Id == accommodationId);
        if (obj is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(obj);
    }

    [HttpPost]
    public IActionResult Delete(Accommodation obj)
    {
        Accommodation? objFromDb = _context.Accommodations.FirstOrDefault(x => x.Id == obj.Id);
        if (objFromDb is not null)
        {
            _context.Accommodations.Remove(objFromDb);
            _context.SaveChanges();
            TempData["success"] = "Broj smještaja je uspješno izbrisan.";
            return RedirectToAction("Index");
        }
        TempData["error"] = "Broj smještaja nije moguće izbrisati.";
        return View();
    }
}