using E_commerce.Core.Mapping;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using E_commerce.Infrastructure.Authentication;

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
                .AddFileServices()
                .AddAuthSystem(configuration)
                .AddHealthCheckServices(configuration);

        return services;
    }

    private static IServiceCollection AddHealthCheckServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("EcommerceDatabase")!, name: "SQLServer")
            .AddRedis(configuration.GetConnectionString("redis")!, name: "Redis");

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

    private static IServiceCollection AddAuthSystem(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.SaveToken = true;
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Key)),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience
            };
        });

        return services;
    }
}
