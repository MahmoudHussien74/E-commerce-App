using E_commerce.Core.Entities.Product;

namespace E_commerce.Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product> 
{
    Task<Product> GetProductById(int id, CancellationToken cancellationToken);
     Task<Product> GetProductIncludeCategoryAndPhotoAsync(int id, CancellationToken cancellationToken);

}
