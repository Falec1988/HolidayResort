using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace HolidayResort.Web.Controllers;

public class AccommodationController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IWebHostEnvironment _webHostEnvironment;

    public AccommodationController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var accommodations = _unitOfWork.Accommodation.GetAll();
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
            if (obj.Image is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\AccommodationImage");

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                obj.Image.CopyTo(fileStream);

                obj.ImageUrl = @"\images\AccommodationImage\" + fileName;
            }
            else
            {
                obj.ImageUrl = "https://placehold.co/600x400";
            }

            _unitOfWork.Accommodation.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "Smještaj je uspješno kreiran.";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    public IActionResult Update(int accommodationId)
    {
        Accommodation? obj = _unitOfWork.Accommodation.Get(u => u.Id == accommodationId);
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
            if (obj.Image is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\AccommodationImage");

                if (!string.IsNullOrEmpty(obj.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,obj.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                obj.Image.CopyTo(fileStream);

                obj.ImageUrl = @"\images\AccommodationImage\" + fileName;
            }

            _unitOfWork.Accommodation.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Smještaj je uspješno uređen.";
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    public IActionResult Delete(int accommodationId)
    {
        Accommodation? obj = _unitOfWork.Accommodation.Get(u => u.Id == accommodationId);
        if (obj is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(obj);
    }

    [HttpPost]
    public IActionResult Delete(Accommodation obj)
    {
        Accommodation? objFromDb = _unitOfWork.Accommodation.Get(d => d.Id == obj.Id);
        if (objFromDb is not null)
        {
            if (!string.IsNullOrEmpty(objFromDb.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Accommodation.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Smještaj je uspješno izbrisan.";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "Smještaj nije moguće izbrisati.";
        return View();
    }
}