using E_commerce.Core.Interfaces;
using E_commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_commerce.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        _context.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

    }


    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default!)
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();

        foreach (var item in includes)
            query = query.Include(item);


        return await query.ToListAsync();

    }
    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default!)
    {
        return await _context.Set<T>().FindAsync(id, cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default!)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }


}
