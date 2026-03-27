namespace E_commerce.Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product> 
{
    Task<Product> GetProductById(int id, CancellationToken cancellationToken);
    Task<Product> GetProductIncludeCategoryAndPhotoAsync(int id, CancellationToken cancellationToken);
    Task<PaginatedList<Product>> GetAllProductsAsync(RequestFilter filters, CancellationToken cancellationToken);
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids);
}
