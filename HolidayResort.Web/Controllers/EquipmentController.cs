using HolidayResort.Application.Interfaces;
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
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var equipments = _unitOfWork.Equipment.GetAll(includeProperties: "Accommodation");
        return View(equipments);
    }

    public IActionResult Create()
    {
        EquipmentVM equipmentVM = new()
        {
            AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
        };
        return View(equipmentVM);
    }

    [HttpPost]
    public IActionResult Create(EquipmentVM obj)
    {

        if (ModelState.IsValid)
        {
            _unitOfWork.Equipment.Add(obj.Equipment);
            _unitOfWork.Save();
            TempData["success"] = "Oprema je uspješno kreirana.";
            return RedirectToAction(nameof(Index));
        }
        
        obj.AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });
        return View(obj);
    }

    public IActionResult Update(int equipmentId)
    {
        EquipmentVM equipmentVM = new()
        {
            AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
            Equipment = _unitOfWork.Equipment.Get(x => x.Id == equipmentId)
        };
        if (equipmentVM.Equipment is null)
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
            _unitOfWork.Equipment.Update(equipmentVM.Equipment);
            _unitOfWork.Save();
            TempData["success"] = "Oprema je uspješno uređena.";
            return RedirectToAction(nameof(Index));
        }
        equipmentVM.AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });
        return View(equipmentVM);
    }

    public IActionResult Delete(int equipmentId)
    {
        EquipmentVM equipmentVM = new()
        {
            AccommodationList = _unitOfWork.Accommodation.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
            Equipment = _unitOfWork.Equipment.Get(x => x.Id == equipmentId)
        };
        if (equipmentVM.Equipment is null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(equipmentVM);
    }

    [HttpPost]
    public IActionResult Delete(EquipmentVM equipmentVM)
    {
        Equipment? objFromDb = _unitOfWork.Equipment
            .Get(x => x.Id == equipmentVM.Equipment.Id);

        if (objFromDb is not null)
        {
            _unitOfWork.Equipment.Remove(objFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Oprema je uspješno izbrisana.";
            return RedirectToAction(nameof(Index));
        }
        TempData["error"] = "Opremu nije moguće izbrisati.";
        return View();
    }
}