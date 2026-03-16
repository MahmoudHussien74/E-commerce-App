using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Category;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Core.Interfaces;

public interface ICategoryService
{
    Task<Result<IReadOnlyList<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<CategoryResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<CategoryResponse>> AddAsync(CategoryRequest category, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
