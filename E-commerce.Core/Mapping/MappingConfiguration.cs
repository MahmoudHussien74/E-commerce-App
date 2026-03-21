using AutoMapper;
using E_commerce.Core.Contracts.Basket;
using E_commerce.Core.Contracts.Category;
using E_commerce.Core.Contracts.Product;
using E_commerce.Core.Entities;
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

        CreateMap<CustomerBasket, CustomerBasketResponse>()
            .ForMember(dest => dest.BasketItems, opt => opt.MapFrom(src => src.basketItems));

        CreateMap<CustomerBasketResponse, CustomerBasket>()
            .ForMember(dest => dest.basketItems, opt => opt.MapFrom(src => src.BasketItems));

        CreateMap<BasketItem, BasketItemResponse>().ReverseMap();
    }
}
