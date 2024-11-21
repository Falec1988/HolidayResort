using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Services.Interface;
using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Services.Implementation;

public class EquipmentService : IEquipmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void CreateEquipment(Equipment equipment)
    {
        ArgumentNullException.ThrowIfNull(equipment);

        _unitOfWork.Equipment.Add(equipment);
        _unitOfWork.Save();
    }

    public bool DeleteEquipment(int id)
    {
        try
        {
            var equipment = _unitOfWork.Equipment.Get(u => u.Id == id);

            if (equipment != null)
            {

                _unitOfWork.Equipment.Remove(equipment);
                _unitOfWork.Save();
                return true;
            }
            else
            {
                throw new InvalidOperationException($"Oprema {id} nije pronađena.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }

    public IEnumerable<Equipment> GetAllEquipments()
    {
        return _unitOfWork.Equipment.GetAll(includeProperties: "Accommodation");
    }

    public Equipment GetEquipmentById(int id)
    {
        return _unitOfWork.Equipment.Get(u => u.Id == id, includeProperties: "Accommodation");
    }

    public void UpdateEquipment(Equipment equipment)
    {
        ArgumentNullException.ThrowIfNull(equipment);

        _unitOfWork.Equipment.Update(equipment);
        _unitOfWork.Save();
    }
}