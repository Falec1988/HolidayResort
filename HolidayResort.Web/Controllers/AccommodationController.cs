using HolidayResort.Application.Services.Interface;
using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HolidayResort.Web.Controllers;

[Authorize]
public class AccommodationController : Controller
{
    private readonly IAccommodationService _accommodationService;

    public AccommodationController(IAccommodationService accommodationService)
    {
        _accommodationService = accommodationService;
    }

    public IActionResult Index()
    {
        var accommodations = _accommodationService.GetAllAccommodations();
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
            _accommodationService.CreateAccommodation(obj);
            TempData["success"] = "Smještaj je uspješno kreiran.";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    public IActionResult Update(int accommodationId)
    {
        Accommodation? obj = _accommodationService.GetAccommodationById(accommodationId);
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
            _accommodationService.UpdateAccommodation(obj);
            TempData["success"] = "Smještaj je uspješno uređen.";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    public IActionResult Delete(int accommodationId)
    {
        Accommodation? obj = _accommodationService.GetAccommodationById(accommodationId);
        if (obj is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(obj);
    }

    [HttpPost]
    public IActionResult Delete(Accommodation obj)
    {
        bool deleted = _accommodationService.DeleteAccommodation(obj.Id);

        if (deleted)
        {
            TempData["success"] = "Smještaj je uspješno izbrisan.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = "Smještaj nije moguće izbrisati.";
        }
        return View();
    }
}