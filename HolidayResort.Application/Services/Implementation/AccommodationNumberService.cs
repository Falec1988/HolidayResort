using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Services.Interface;
using HolidayResort.Domain.Entities;

namespace HolidayResort.Application.Services.Implementation;

public class AccommodationNumberService : IAccommodationNumberService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccommodationNumberService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public bool CheckAccommodationNumberExists(int accommodationNo)
    {
        return _unitOfWork.AccommodationNumber.Any(x => x.AccommodationNo == accommodationNo);
    }

    public void CreateAccommodationNumber(AccommodationNumber accommodationNumber)
    {
        _unitOfWork.AccommodationNumber.Add(accommodationNumber);
        _unitOfWork.Save();
    }

    public bool DeleteAccommodationNumber(int id)
    {
        try
        {
            AccommodationNumber? objFromDb = _unitOfWork.AccommodationNumber.Get(d => d.AccommodationNo == id);
            if (objFromDb is not null)
            {
                _unitOfWork.AccommodationNumber.Remove(objFromDb);
                _unitOfWork.Save();

                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public AccommodationNumber GetAccommodationNumberById(int id)
    {
        return _unitOfWork.AccommodationNumber.Get(x => x.AccommodationNo == id, includeProperties: "Accommodation");
    }

    public IEnumerable<AccommodationNumber> GetAllAccommodationNumbers()
    {
        return _unitOfWork.AccommodationNumber.GetAll(includeProperties: "Accommodation");
    }

    public void UpdateAccommodationNumber(AccommodationNumber accommodationNumber)
    {
        _unitOfWork.AccommodationNumber.Update(accommodationNumber);
        _unitOfWork.Save();
    }
}