using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HolidayResort.Web.Controllers;

public class AccommodationController : Controller
{
    private readonly IAccommodationRepository _accommodationRepo;

    public AccommodationController(IAccommodationRepository accommodationRepo)
    {
        _accommodationRepo = accommodationRepo;
    }

    public IActionResult Index()
    {
        var accommodations = _accommodationRepo.GetAll();
        return View(accommodations);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Accommodation obj)
    {
        if (obj.Name == obj.Description)
        {
            ModelState.AddModelError("Name", "Naziv i Opis ne smiju imati isti tekst!");
        }
        if (ModelState.IsValid)
        {
            _accommodationRepo.Add(obj);
            _accommodationRepo.Save();
            TempData["success"] = "Smještaj je uspješno kreiran.";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    public IActionResult Update(int accommodationId)
    {
        Accommodation? obj = _accommodationRepo.Get(u => u.Id == accommodationId);
        if (obj is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(obj);
    }

    [HttpPost]
    public IActionResult Update(Accommodation obj)
    {
        if (ModelState.IsValid && obj.Id > 0)
        {
            _accommodationRepo.Update(obj);
            _accommodationRepo.Save();
            TempData["success"] = "Smještaj je uspješno uređen.";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    public IActionResult Delete(int accommodationId)
    {
        Accommodation? obj = _accommodationRepo.Get(u => u.Id == accommodationId);
        if (obj is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(obj);
    }

    [HttpPost]
    public IActionResult Delete(Accommodation obj)
    {
        Accommodation? objFromDb = _accommodationRepo.Get(d => d.Id == obj.Id);
        if (objFromDb is not null)
        {
            _accommodationRepo.Remove(objFromDb);
            _accommodationRepo.Save();
            TempData["success"] = "Smještaj je uspješno izbrisan.";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "Smještaj nije moguće izbrisati.";
        return View();
    }
}