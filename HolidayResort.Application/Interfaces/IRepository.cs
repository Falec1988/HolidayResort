using HolidayResort.Domain.Entities;
using System.Linq.Expressions;

namespace HolidayResort.Application.Interfaces;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll(Expression<Func<Accommodation, bool>>? filter = null, string? includeProperties = null);

    T Get(Expression<Func<Accommodation, bool>>? filter, string? includeProperties = null);

    void Add(T entity);

    void Remove(T entity);
}