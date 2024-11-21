using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HolidayResort.Web.ViewModels;

public class AccommodationNumberVM
{
    public AccommodationNumber? AccommodationNumber { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem>? AccommodationList { get; set; }
}