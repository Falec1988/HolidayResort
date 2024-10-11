﻿using System.Linq.Expressions;

namespace HolidayResort.Application.Interfaces;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

    T Get(Expression<Func<T, bool>>? filter, string? includeProperties = null);

    void Add(T entity);

    void Remove(T entity);
}