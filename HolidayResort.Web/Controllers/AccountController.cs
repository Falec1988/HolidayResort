using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using HolidayResort.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HolidayResort.Web.Controllers;

public class AccountController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public IActionResult Login(string returnUrl = null)
    {
        returnUrl??= Url.Content("~/");

        LoginVM loginVM = new()
        {
            RedirectUrl = returnUrl
        };

        return View(loginVM);
    }

    public IActionResult Register()
    {
        return View();
    }
}
