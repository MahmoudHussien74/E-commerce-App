using System.Linq.Expressions;

namespace E_commerce.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }
    public void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Remove(entity);
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
    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();
        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default!)
        => await _context.Set<T>().FindAsync([id], cancellationToken);

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }
    public async Task<IReadOnlyList<T>> GetListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsNoTracking().Where(predicate);
        foreach (var include in includes)
            query = query.Include(include);

        return await query.ToListAsync(cancellationToken);
    }
    public IQueryable<T> GetQueryable()
    {
        return  _context.Set<T>().AsQueryable();
    }
    public IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();

        foreach (var include in includes)
            query = query.Include(include);

        return query;
    }
}
