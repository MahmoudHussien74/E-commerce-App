using System.Linq.Expressions;

namespace E_commerce.Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken =default!);
    Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default!);
     Task AddRangeAsync(List<T> entity, CancellationToken cancellationToken);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default!);

    Task AddAsync(T entity, CancellationToken cancellationToken = default!);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default!);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default!);

}
