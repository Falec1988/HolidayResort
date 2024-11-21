using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Interfaces;

public interface IEquipmentRepository : IRepository<Equipment>
{
    void Update(Equipment entity);
}