using AutoMapper;
using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Common;
using E_commerce.Core.Contracts.Product;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product> 
{
    Task<Product> GetProductById(int id, CancellationToken cancellationToken);
    Task<Product> GetProductIncludeCategoryAndPhotoAsync(int id, CancellationToken cancellationToken);
    Task<PaginatedList<Product>> GetAllProductsAsync(RequestFilter filters, CancellationToken cancellationToken);
}
