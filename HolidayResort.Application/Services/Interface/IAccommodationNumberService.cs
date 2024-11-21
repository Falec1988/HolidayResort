using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Services.Interface;

public interface IAccommodationNumberService
{
    IEnumerable<AccommodationNumber> GetAllAccommodationNumbers();

    AccommodationNumber GetAccommodationNumberById(int id);

    void CreateAccommodationNumber(AccommodationNumber accommodationNumber);

    void UpdateAccommodationNumber(AccommodationNumber accommodationNumber);

    bool DeleteAccommodationNumber(int id);

    bool CheckAccommodationNumberExists(int accommodationNo);
}