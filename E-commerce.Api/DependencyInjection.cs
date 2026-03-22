using E_commerce.Core.Mapping;
using Microsoft.Extensions.FileProviders;

namespace E_commerce.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCoreDependencies()
                .AddInfrastructureDependencies(configuration)
                .AddMappingServices()
                .AddSwaggerServices()
                .AddControllerServices()
                .AddGlobalExceptionServices()
                .AddFileServices();

        return services;
    }

    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    private static IServiceCollection AddControllerServices(this IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        return services;
    }

    private static IServiceCollection AddMappingServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(typeof(MappingConfiguration).Assembly);
        });

        return services;
    }

    private static IServiceCollection AddGlobalExceptionServices(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }

    private static IServiceCollection AddFileServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
        );

        services.AddSingleton<IImageMangementService, ImageMangementService>();

        return services;
    }
}
