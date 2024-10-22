using HolidayResort.Application.Services.Interface;
using HolidayResort.Domain.Entities;
using HolidayResort.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HolidayResort.Web.Controllers;

public class AccommodationNumberController : Controller
{
    private readonly IAccommodationService _accommodationService;
    private readonly IAccommodationNumberService _accommodationNumberService;

    public AccommodationNumberController(IAccommodationService accommodationService, IAccommodationNumberService accommodationNumberService)
    {
        _accommodationService = accommodationService;
        _accommodationNumberService = accommodationNumberService;
    }

    public IActionResult Index()
    {
        var accommodationNumbers = _accommodationNumberService.GetAllAccommodationNumbers();
        return View(accommodationNumbers);
    }

    public IActionResult Create()
    {
        AccommodationNumberVM accommodationNumberVM = new()
        {
            AccommodationList = _accommodationService.GetAllAccommodations().Select(x => new SelectListItem
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
        bool accommodationNoExists = _accommodationNumberService.CheckAccommodationNumberExists(obj.AccommodationNumber.AccommodationNo);

        if (ModelState.IsValid && !accommodationNoExists)
        {
            _accommodationNumberService.CreateAccommodationNumber(obj.AccommodationNumber);
            TempData["success"] = "Broj smještaja je uspješno kreiran.";
            return RedirectToAction(nameof(Index));
        }
        if (accommodationNoExists)
        {
            TempData["error"] = "Broj smještaja već postoji.";
        };
        obj.AccommodationList = _accommodationService.GetAllAccommodations().Select(x => new SelectListItem
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
            AccommodationList = _accommodationService.GetAllAccommodations().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
            AccommodationNumber = _accommodationNumberService.GetAccommodationNumberById(accommodationNumberId)
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
            _accommodationNumberService.UpdateAccommodationNumber(accommodationNumberVM.AccommodationNumber);
            TempData["success"] = "Broj smještaja je uspješno uređen.";
            return RedirectToAction(nameof(Index));
        }
        accommodationNumberVM.AccommodationList = _accommodationService.GetAllAccommodations().Select(x => new SelectListItem
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
            AccommodationList = _accommodationService.GetAllAccommodations().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
            AccommodationNumber = _accommodationNumberService.GetAccommodationNumberById(accommodationNumberId)
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
        AccommodationNumber? objFromDb = _accommodationNumberService.GetAccommodationNumberById(accommodationNumberVM.AccommodationNumber.AccommodationNo);

        if (objFromDb is not null)
        {
            _accommodationNumberService.DeleteAccommodationNumber(objFromDb.AccommodationNo);
            TempData["success"] = "Broj smještaja je uspješno izbrisan.";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "Broj smještaja nije moguće izbrisati.";
        return View();
    }
}