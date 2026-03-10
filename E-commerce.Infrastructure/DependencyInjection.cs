using E_commerce.Core.Interfaces;
using E_commerce.Infrastructure.Data;
using E_commerce.Infrastructure.Repositries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_commerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("EcommerceDatabase"));
        });
        return services;
    }
}
