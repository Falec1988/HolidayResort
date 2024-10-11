using HolidayResort.Domain.Entities;
using System.Linq.Expressions;

namespace HolidayResort.Application.Interfaces;

public interface IAccommodationRepository
{
    IEnumerable<Accommodation> GetAll(Expression<Func<Accommodation,bool>>? filter = null, string? includeProperties = null);

    IEnumerable<Accommodation> Get(Expression<Func<Accommodation, bool>>? filter, string? includeProperties = null);

    void Create(Accommodation entity);

    void Update(Accommodation entity);

    void Delete(Accommodation entity);

    void Save(Accommodation entity);
}