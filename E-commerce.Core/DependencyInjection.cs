using E_commerce.Core.Interfaces;
using E_commerce.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace E_commerce.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBasketService, BasketService>();

        return services;
    }
}
