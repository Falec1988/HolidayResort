using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using HolidayResort.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HolidayResort.Web.Controllers;

public class AccommodationNumberController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AccommodationNumberController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var accommodationNumbers = _unitOfWork.AccommodationNumber.GetAll(includeProperties: "Accommodation");
        return View(accommodationNumbers);
    }

    public IActionResult Create()
    {
        AccommodationNumberVM accommodationNumberVM = new()
        {
            AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
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
        bool accommodationNoExists = _unitOfWork.AccommodationNumber.Any(x => x.AccommodationNo == obj.AccommodationNumber.AccommodationNo);

        if (ModelState.IsValid && !accommodationNoExists)
        {
            _unitOfWork.AccommodationNumber.Add(obj.AccommodationNumber);
            _unitOfWork.Save();
            TempData["success"] = "Broj smještaja je uspješno kreiran.";
            return RedirectToAction(nameof(Index));
        }
        if (accommodationNoExists)
        {
            TempData["error"] = "Broj smještaja već postoji.";
        };
        obj.AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
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
            AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
            AccommodationNumber = _unitOfWork.AccommodationNumber.Get(x => x.AccommodationNo == accommodationNumberId)
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
            _unitOfWork.AccommodationNumber.Update(accommodationNumberVM.AccommodationNumber);
            _unitOfWork.Save();
            TempData["success"] = "Broj smještaja je uspješno uređen.";
            return RedirectToAction(nameof(Index));
        }
        accommodationNumberVM.AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
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
            AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
            AccommodationNumber = _unitOfWork.AccommodationNumber.Get(x => x.AccommodationNo == accommodationNumberId)
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
        AccommodationNumber? objFromDb = _unitOfWork.AccommodationNumber
            .Get(x => x.AccommodationNo == accommodationNumberVM.AccommodationNumber.AccommodationNo);

        if (objFromDb is not null)
        {
            _unitOfWork.AccommodationNumber.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Broj smještaja je uspješno izbrisan.";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "Broj smještaja nije moguće izbrisati.";
        return View();
    }
}