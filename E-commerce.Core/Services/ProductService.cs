using E_commerce.Core.Common;
using E_commerce.Core.Entities.Product;
using E_commerce.Core.Errors;
using E_commerce.Core.Interfaces;

namespace E_commerce.Core.Services;

public class ProductService(IUnitOfWork unitOfWork) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<Product>>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.ProductRepository.GetAllAsync(cancellationToken);
        return Result.Success(products);
    }

    public async Task<Result<Product?>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, cancellationToken);
        
        if (product is null)
            return Result.Failure<Product?>(ProductErrors.NotFound);

        return Result.Success<Product?>(product);
    }
}
