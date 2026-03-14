using E_commerce.Core.Entities.Product;
using E_commerce.Core.Interfaces;

namespace E_commerce.Core.Services;

public class ProductService(IUnitOfWork unitOfWork) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ProductRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ProductRepository.GetByIdAsync(id, cancellationToken);
    }
}
