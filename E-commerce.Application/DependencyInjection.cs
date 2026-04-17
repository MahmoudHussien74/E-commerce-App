using E_commerce.Application.Abstractions.Authentication;
using E_commerce.Application.Abstractions.Services;
using E_commerce.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace E_commerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IDeliveryMethodService, DeliveryMethodService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
