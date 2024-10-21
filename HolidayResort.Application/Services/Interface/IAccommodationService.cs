using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Services.Interface;

public interface IAccommodationService
{
    IEnumerable<Accommodation> GetAllAccommodations();

    Accommodation GetAccommodationById(int id);

    void CreateAccommodation(Accommodation accommodation);

    void UpdateAccommodation(Accommodation accommodation);

    bool DeleteAccommodation(int id);

    IEnumerable<Accommodation> GetAccommodationsAvailabilityByDate(int nights, DateOnly checkInDate);
}