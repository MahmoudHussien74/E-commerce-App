using E_commerce.Core.Entities.Order;
using E_commerce.Core.Contracts.Order;
using E_commerce.Core.Entities.Product;
using E_commerce.Core.Contracts.Product;
using E_commerce.Core.Contracts.Category;
using AutoMapper;
using E_commerce.Core.Entities;

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

        // Order Mappings
        CreateMap<DeliveryMethod, DeliveryMethodResponse>();
        CreateMap<AddressDto, ShippingAddress>().ReverseMap();
        CreateMap<Orders, OrderResponse>()
            .ForMember(dest => dest.DeliveryMethod, opt => opt.MapFrom(src => src.DeliveryMethod.ShortName))
            .ForMember(dest => dest.ShippingPrice, opt => opt.MapFrom(src => src.DeliveryMethod.Price))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.SubTotal + src.DeliveryMethod.Price));
        CreateMap<OrderItem, OrderItemResponse>().ReverseMap();
        CreateMap<Product, ProductResponse>();
    }
}
