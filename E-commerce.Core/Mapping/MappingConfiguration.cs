using AutoMapper;
using E_commerce.Core.Contracts.Category;
using E_commerce.Core.Contracts.Product;
using E_commerce.Core.Entities.Product;

namespace E_commerce.Core.Mapping;

public class MappingConfiguration : Profile
{
    public MappingConfiguration()
    {
        CreateMap<CategoryRequest, Category>();
        CreateMap<Category, CategoryResponse>().ReverseMap();
        CreateMap<UpdateCategoryRequest, Category>();


        CreateMap<Photo, ProductImageResponse>()
            .ForMember(dest => dest.ProductId, src => src.MapFrom(x => x.ProductId))
            .ForMember(dest => dest.Url, src => src.MapFrom(x => x.ImageName));

        CreateMap<Product, ProductResponse>();
        CreateMap<ProductRequest, Product>();



    }
}
