using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    void Update(Booking entity);
}