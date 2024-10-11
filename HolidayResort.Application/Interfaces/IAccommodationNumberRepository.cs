using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Interfaces;

public interface IAccommodationNumberRepository : IRepository<AccommodationNumber>
{
    void Update(AccommodationNumber entity);
}