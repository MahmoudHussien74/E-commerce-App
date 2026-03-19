using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Product;

namespace E_commerce.Core.Interfaces;

public interface IProductService
{
    Task<Result<List<ProductResponse>>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> AddAsync(ProductRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(int id,ProductRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);



}
