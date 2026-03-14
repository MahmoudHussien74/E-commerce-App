using AutoMapper;
using E_commerce.Core.Contracts.Category;
using E_commerce.Core.Entities.Product;
using E_commerce.Core.Interfaces;

namespace E_commerce.Core.Services;

public class CategoryService(IUnitOfWork unitOfWork,IMapper mapper): ICategoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<Category> AddAsync(CategoryRequest category, CancellationToken cancellationToken = default)
    {
        var newCategory = _mapper.Map<Category>(category);
        

         await _unitOfWork.CategoryRepository.AddAsync(newCategory,cancellationToken);

        return newCategory; 
    }
    public async Task<bool> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var categoryIsExsist = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id,cancellationToken);

        if (categoryIsExsist is null)
            return false;


        _mapper.Map(request,categoryIsExsist);
        

        await _unitOfWork.CategoryRepository.UpdateAsync(categoryIsExsist, cancellationToken);
        return true;
    }
    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    => await _unitOfWork.CategoryRepository.GetAllAsync(cancellationToken);



    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
     =>  await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {

        var categoryIsExsist = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);

        if (categoryIsExsist is null)
            return false;

        await _unitOfWork.CategoryRepository.DeleteAsync(id,cancellationToken);

        return true;

    }
}
