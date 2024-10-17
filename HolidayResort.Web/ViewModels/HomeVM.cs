using HolidayResort.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace HolidayResort.Web.ViewModels;

public class HomeVM
{
    public IEnumerable<Accommodation>? AccommodationList { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public int Nights { get; set; }
}