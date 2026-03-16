using AutoMapper;
using E_commerce.Core.Contracts.Category;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Core.Mapping;

public class CategoryMapping:Profile
{
    public CategoryMapping()
    {
        CreateMap<CategoryRequest, Category>();
        CreateMap<Category, CategoryResponse>().ReverseMap();
        CreateMap<UpdateCategoryRequest, Category>();
    }
}
