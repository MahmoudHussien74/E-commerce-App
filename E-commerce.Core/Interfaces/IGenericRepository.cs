using System.Linq;
using System.Linq.Expressions;

namespace E_commerce.Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken =default!);
    Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default!, params Expression<Func<T, object>>[] includes);
     Task AddRangeAsync(List<T> entity, CancellationToken cancellationToken);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default!);
    IQueryable<T> GetQueryable();
    IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    Task AddAsync(T entity, CancellationToken cancellationToken = default!);
    void Update(T entity);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default!);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default!);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default!);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default!);

    Task<IReadOnlyList<T>> GetListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
}
