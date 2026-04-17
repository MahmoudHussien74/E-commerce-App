using E_commerce.Application.Abstractions.Persistence;
using E_commerce.Application.Abstractions.Services;
using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Category;
using E_commerce.Application.Errors;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Application.Services;

internal sealed class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<Result<CategoryResponse>> AddAsync(CategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        unitOfWork.CategoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(category));
    }

    public async Task<Result> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound);
        }

        category.Name = request.Name;
        category.Description = request.Description;
        unitOfWork.CategoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await unitOfWork.CategoryRepository.GetAllAsync(cancellationToken);
        return Result.Success<IReadOnlyList<CategoryResponse>>(categories.Select(Map).ToList());
    }

    public async Task<Result<CategoryResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
        return category is null
            ? Result.Failure<CategoryResponse>(CategoryErrors.NotFound)
            : Result.Success(Map(category));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound);
        }

        await unitOfWork.CategoryRepository.DeleteAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static CategoryResponse Map(Category category) => new(category.Id, category.Name, category.Description);
}
