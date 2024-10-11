namespace HolidayResort.Application.Interfaces;

public interface IUnitOfWork
{
    IAccommodationRepository Accommodation { get; }

    void Save();
}