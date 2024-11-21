using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Interfaces;

public interface IAccommodationRepository : IRepository<Accommodation>
{
    void Update(Accommodation entity);
}