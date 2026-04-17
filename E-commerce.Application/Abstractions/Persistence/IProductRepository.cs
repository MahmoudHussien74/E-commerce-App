using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Common;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Application.Abstractions.Persistence;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product?> GetProductWithPhotosAsync(int id, CancellationToken cancellationToken = default);
    Task<PaginatedList<Product>> GetAllProductsAsync(RequestFilter filter, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
