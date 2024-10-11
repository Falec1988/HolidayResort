using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using System.Linq.Expressions;

namespace HolidayResort.Infrastructure.Repositories;

public class AccommodationRepository : IAccommodationRepository
{
    public void Create(Accommodation entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Accommodation entity)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Accommodation> Get(Expression<Func<Accommodation, bool>>? filter, string? includeProperties = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Accommodation> GetAll(Expression<Func<Accommodation, bool>>? filter = null, string? includeProperties = null)
    {
        throw new NotImplementedException();
    }

    public void Save(Accommodation entity)
    {
        throw new NotImplementedException();
    }

    public void Update(Accommodation entity)
    {
        throw new NotImplementedException();
    }
}