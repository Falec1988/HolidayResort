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
    public IActionResult Update(AccommodationNumberVM accommodationNumberVM)
    {
        if (ModelState.IsValid)
        {
            _context.AccommodationNumbers.Update(accommodationNumberVM.AccommodationNumber);
            _context.SaveChanges();
            TempData["success"] = "Broj smještaja je uspješno uređen.";
            return RedirectToAction("Index");
        }

        accommodationNumberVM.AccommodationList = _context.Accommodations.ToList().Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });

        return View(accommodationNumberVM);
    }

    public IActionResult Delete(int accommodationNumberId)
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
    public IActionResult Delete(AccommodationNumberVM accommodationNumberVM)
    {
        AccommodationNumber? objFromDb = _context.AccommodationNumbers
            .FirstOrDefault(x => x.AccommodationNo == accommodationNumberVM.AccommodationNumber.AccommodationNo);
        if (objFromDb is not null)
        {
            _context.AccommodationNumbers.Remove(objFromDb);
            _context.SaveChanges();
            TempData["success"] = "Broj smještaja je uspješno izbrisan.";
            return RedirectToAction("Index");
        }
        TempData["error"] = "Broj smještaja nije moguće izbrisati.";
        return View();
    }
}