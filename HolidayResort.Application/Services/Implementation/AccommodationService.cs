using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Services.Interface;
using HolidayResort.Application.Utility;
using HolidayResort.Domain.Entities;
using Microsoft.AspNetCore.Hosting;

namespace HolidayResort.Application.Services.Implementation;

public class AccommodationService : IAccommodationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AccommodationService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public void CreateAccommodation(Accommodation accommodation)
    {
        if (accommodation.Image is not null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(accommodation.Image.FileName);
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\AccommodationImage");

            using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
            accommodation.Image.CopyTo(fileStream);

            accommodation.ImageUrl = @"\images\AccommodationImage\" + fileName;
        }
        else
        {
            accommodation.ImageUrl = "https://placehold.co/600x400";
        }

        _unitOfWork.Accommodation.Add(accommodation);
        _unitOfWork.Save();
    }

    public bool DeleteAccommodation(int id)
    {
        try
        {
            Accommodation? objFromDb = _unitOfWork.Accommodation.Get(d => d.Id == id);
            if (objFromDb is not null)
            {
                if (!string.IsNullOrEmpty(objFromDb.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.Accommodation.Remove(objFromDb);
                _unitOfWork.Save();
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Accommodation GetAccommodationById(int id)
    {
        return _unitOfWork.Accommodation.Get(x => x.Id == id, includeProperties: "AccommodationEquipment");
    }

    public IEnumerable<Accommodation> GetAccommodationsAvailabilityByDate(int nights, DateOnly checkInDate)
    {
        var accommodationList = _unitOfWork.Accommodation.GetAll(includeProperties: "AccommodationEquipment").ToList();

        var accommodationNoList = _unitOfWork.AccommodationNumber.GetAll().ToList();

        var bookedAccommodation = _unitOfWork.Booking.GetAll(x => x.Status == SD.StatusApproved ||
        x.Status == SD.StatusCheckedIn).ToList();

        foreach (var accommodation in accommodationList)
        {
            int roomAvailable = SD.AccommodationRoomsAvailableCount
                (accommodation.Id, accommodationNoList, checkInDate, nights, bookedAccommodation);

            accommodation.IsAvailable = roomAvailable > 0 ? true : false;
        }
        return accommodationList;
    }

    public IEnumerable<Accommodation> GetAllAccommodations()
    {
        return _unitOfWork.Accommodation.GetAll(includeProperties: "AccommodationEquipment");
    }

    public bool IsAccommodationAvailableByDate(int accommodationId, int nights, DateOnly checkInDate)
    {
        var accommodationNoList = _unitOfWork.AccommodationNumber.GetAll().ToList();

        var bookedAccommodation = _unitOfWork.Booking.GetAll(x => x.Status == SD.StatusApproved ||
        x.Status == SD.StatusCheckedIn).ToList();

        int roomAvailable = SD.AccommodationRoomsAvailableCount
                (accommodationId, accommodationNoList, checkInDate, nights, bookedAccommodation);

        return roomAvailable > 0;
    }

    public void UpdateAccommodation(Accommodation accommodation)
    {
        if (accommodation.Image is not null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(accommodation.Image.FileName);
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\AccommodationImage");

            if (!string.IsNullOrEmpty(accommodation.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, accommodation.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
            accommodation.Image.CopyTo(fileStream);

            accommodation.ImageUrl = @"\images\AccommodationImage\" + fileName;
        }
        _unitOfWork.Accommodation.Update(accommodation);
        _unitOfWork.Save();
    }
}