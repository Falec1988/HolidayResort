using HolidayResort.Application.Services.Interface;
using HolidayResort.Application.Utility;
using HolidayResort.Domain.Entities;
using HolidayResort.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HolidayResort.Web.Controllers;

[Authorize(Roles = SD.Role_Admin)]
public class EquipmentController : Controller
{
    private readonly IAccommodationService _accommodationService;

    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IAccommodationService accommodationService, IEquipmentService equipmentService)
    {
        _accommodationService = accommodationService;
        _equipmentService = equipmentService;
    }

    public IActionResult Index()
    {
        var equipment = _equipmentService.GetAllEquipments();
        return View(equipment);
    }

    public IActionResult Create()
    {
        EquipmentVM equipmentVM = new()
        {
            AccommodationList = _accommodationService.GetAllAccommodations().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
        return View(equipmentVM);
    }

    [HttpPost]
    public IActionResult Create(EquipmentVM obj)
    {
        if (ModelState.IsValid)
        {
            _equipmentService.CreateEquipment(obj.Equipment);
            TempData["success"] = "Oprema je uspješno kreirana.";
            return RedirectToAction(nameof(Index));
        }
        
        obj.AccommodationList = _accommodationService.GetAllAccommodations().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });
        return View(obj);
    }

    public IActionResult Update(int equipmentId)
    {
        EquipmentVM equipmentVM = new()
        {
            AccommodationList = _accommodationService.GetAllAccommodations().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            
            Equipment = _equipmentService.GetEquipmentById(equipmentId)
        };

        if (equipmentVM.Equipment == null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(equipmentVM);
    }

    [HttpPost]
    public IActionResult Update(EquipmentVM equipmentVM)
    {
        if (ModelState.IsValid)
        {
            _equipmentService.UpdateEquipment(equipmentVM.Equipment);
            TempData["success"] = "Oprema je uspješno uređena.";
            return RedirectToAction(nameof(Index));
        }

        equipmentVM.AccommodationList = _accommodationService.GetAllAccommodations().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });
        return View(equipmentVM);
    }

    public IActionResult Delete(int equipmentId)
    {
        EquipmentVM equipmentVM = new()
        {
            AccommodationList = _accommodationService.GetAllAccommodations().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
             
            Equipment = _equipmentService.GetEquipmentById(equipmentId)
            };

        if (equipmentVM.Equipment == null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(equipmentVM);
    }

    [HttpPost]
    public IActionResult Delete(EquipmentVM equipmentVM)
    {
        Equipment? objFromDb = _equipmentService.GetEquipmentById(equipmentVM.Equipment.Id);
        if (objFromDb is not null)
        {
            _equipmentService.DeleteEquipment(objFromDb.Id);
            TempData["success"] = "Oprema uspješno obrisana.";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "Opremu nije moguće izbrisati.";

        return View();
    }
}