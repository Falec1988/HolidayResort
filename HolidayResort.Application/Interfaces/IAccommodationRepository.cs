using HolidayResort.Domain.Entities;
using System.Linq.Expressions;

namespace HolidayResort.Application.Interfaces;

public interface IAccommodationRepository
{
    IEnumerable<Accommodation> GetAll(Expression<Func<Accommodation,bool>>? filter = null, string? includeProperties = null);

    Accommodation Get(Expression<Func<Accommodation, bool>>? filter, string? includeProperties = null);

    void Add(Accommodation entity);

    void Update(Accommodation entity);

    void Remove(Accommodation entity);

    void Save();
}