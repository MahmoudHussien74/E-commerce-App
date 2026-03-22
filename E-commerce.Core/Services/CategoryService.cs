namespace E_commerce.Core.Services;

public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<CategoryResponse>> AddAsync(CategoryRequest category, CancellationToken cancellationToken = default)
    {
        var newCategory = _mapper.Map<Category>(category);

        await _unitOfWork.CategoryRepository.AddAsync(newCategory, cancellationToken);


        return Result.Success(_mapper.Map<CategoryResponse>(newCategory));
    }
    public async Task<Result> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var categoryIsExsist = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (categoryIsExsist is null)
            return Result.Failure(CategoryErrors.NotFound);

        _mapper.Map(request, categoryIsExsist);

        await _unitOfWork.CategoryRepository.UpdateAsync(categoryIsExsist, cancellationToken);
        return Result.Success();
    }
    public async Task<Result<IReadOnlyList<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.CategoryRepository.GetAllAsync(cancellationToken);
        return Result.Success(_mapper.Map<IReadOnlyList<CategoryResponse>>(categories));
    }

    public async Task<Result<CategoryResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);

        if (category is null)
            return Result.Failure<CategoryResponse>(CategoryErrors.NotFound);

        return Result.Success(_mapper.Map<CategoryResponse>(category));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var categoryIsExsist = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);

        if (categoryIsExsist is null)
            return Result.Failure(CategoryErrors.NotFound);

        await _unitOfWork.CategoryRepository.DeleteAsync(id, cancellationToken);

        return Result.Success();
    }
}
