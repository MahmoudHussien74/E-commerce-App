using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Common;
using E_commerce.Application.Contracts.Product;

namespace E_commerce.Application.Abstractions.Services;

public interface IProductService
{
    Task<Result<PaginatedList<ProductResponse>>> GetAllProductsAsync(RequestFilter? filter, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> AddAsync(ProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
