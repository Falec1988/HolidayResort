using HolidayResort.Application.Interfaces;
using HolidayResort.Domain.Entities;
using HolidayResort.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HolidayResort.Infrastructure.Repositories;

public class AccommodationRepository : IAccommodationRepository
{
    private readonly ApplicationDbContext _context;

    public AccommodationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Accommodation entity)
    {
        _context.Add(entity);
    }

    public void Remove(Accommodation entity)
    {
        _context?.Remove(entity);
    }

    public Accommodation Get(Expression<Func<Accommodation, bool>>? filter, string? includeProperties = null)
    {
        IQueryable<Accommodation> query = _context.Set<Accommodation>();

        if (filter is not null)
        {
            query = query.Where(filter);
        }
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return query.FirstOrDefault();
    }

    public IEnumerable<Accommodation> GetAll(Expression<Func<Accommodation, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<Accommodation> query = _context.Set<Accommodation>();

        if (filter is not null)
        {
            query = query.Where(filter);
        }
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties
                .Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return query.ToList();
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public void Update(Accommodation entity)
    {
        _context.Accommodations.Update(entity);
    }
}