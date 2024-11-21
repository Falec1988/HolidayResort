namespace HolidayResort.Application.Interfaces;

public interface IUnitOfWork
{
    IAccommodationRepository Accommodation { get; }

    IAccommodationNumberRepository AccommodationNumber { get; }

    IApplicationUserRepository User { get; }

    IBookingRepository Booking { get; }

    IEquipmentRepository Equipment { get; }

    void Save();
}