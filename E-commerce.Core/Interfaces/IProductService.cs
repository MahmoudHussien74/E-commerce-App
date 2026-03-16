using E_commerce.Core.Common;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Core.Interfaces;

public interface IProductService
{
    Task<Result<IReadOnlyList<Product>>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<Result<Product?>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);

}
