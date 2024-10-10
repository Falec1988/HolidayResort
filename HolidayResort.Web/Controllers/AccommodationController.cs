﻿using HolidayResort.Domain.Entities;
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
            _context.Accommodations.Add(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View();
    }

    public IActionResult Update(int accommodationId)
    {
        Accommodation? obj = _context.Accommodations.FirstOrDefault(u => u.Id == accommodationId);
        if (obj == null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(obj);
    }
}