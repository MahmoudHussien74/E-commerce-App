using System.Linq.Expressions;

namespace E_commerce.Application.Abstractions.Persistence;

public interface IGenericRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> GetListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
