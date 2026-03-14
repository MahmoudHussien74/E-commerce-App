using E_commerce.Core.Contracts.Category;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Core.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Category> AddAsync(CategoryRequest category, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);


}
