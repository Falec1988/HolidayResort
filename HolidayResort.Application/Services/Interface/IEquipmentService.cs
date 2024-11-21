using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Services.Interface;

public interface IEquipmentService
{
    IEnumerable<Equipment> GetAllEquipments();

    void CreateEquipment(Equipment equipment);

    void UpdateEquipment(Equipment equipment);

    Equipment GetEquipmentById(int id);

    bool DeleteEquipment(int id);
}