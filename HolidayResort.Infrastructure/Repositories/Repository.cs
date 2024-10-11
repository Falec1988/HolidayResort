using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using System.Linq.Expressions;

namespace HolidayResort.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    public void Add(T entity)
    {
        throw new NotImplementedException();
    }

    public T Get(Expression<Func<Accommodation, bool>>? filter, string? includeProperties = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T> GetAll(Expression<Func<Accommodation, bool>>? filter = null, string? includeProperties = null)
    {
        throw new NotImplementedException();
    }

    public void Remove(T entity)
    {
        throw new NotImplementedException();
    }
}