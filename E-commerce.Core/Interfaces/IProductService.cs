using E_commerce.Core.Contracts.Category;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Core.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);

}
