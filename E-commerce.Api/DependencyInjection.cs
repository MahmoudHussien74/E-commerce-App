using E_commerce.Infrastructure.Authentication;
using E_commerce.Infrastructure.Authentication.Permissions;
using E_commerce.Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace E_commerce.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationDependencies()
                .AddInfrastructureDependencies(configuration)
                .AddSwaggerServices()
                .AddControllerServices()
                .AddGlobalExceptionServices()
                .AddFileServices()
                .AddAuthSystem(configuration)
                .AddRateLimiterServices()
                .AddHealthCheckServices(configuration);

        return services;
    }

    public static IServiceCollection AddRateLimiterServices(this IServiceCollection services)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            // Return 429 Too Many Requests when limit is exceeded
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // ── ipLimiter ────────────────────────────────────────────────────
            // Applied on: Login, Register, RefreshToken
            // Purpose: Block brute-force attacks and registration spam
            // Limit: 10 requests / 1 minute per IP address
            rateLimiterOptions.AddPolicy("ipLimiter", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }
                )
            );

            // ── userLimiter ──────────────────────────────────────────────────
            // Applied on: CreatePayment, CreateOrder, CreateBasket
            // Purpose: Prevent per-user abuse of write-heavy endpoints
            // Limit: 30 requests / 1 minute per authenticated userId (falls back to IP)
            rateLimiterOptions.AddPolicy("userLimiter", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.GetUserId()
                                  ?? httpContext.Connection.RemoteIpAddress?.ToString()
                                  ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 30,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }
                )
            );

            // ── GlobalLimiter (Concurrency) ──────────────────────────────────
            // Applied automatically to ALL endpoints
            // Purpose: Prevent server overload under high traffic
            // Limit: Max 1000 simultaneous requests, queue up to 100
            rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                _ => RateLimitPartition.GetConcurrencyLimiter(
                    partitionKey: "global",
                    factory: _ => new ConcurrencyLimiterOptions
                    {
                        PermitLimit = 1000,
                        QueueLimit = 100,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }
                )
            );
        });

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
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "E-Commerce API",
                Version = "v1",
                Description = "A production-ready E-Commerce REST API built with ASP.NET Core 9 and Clean Architecture. " +
                              "Implements a service-based pattern with permission-based authorization (JWT). " +
                              "Features include: product &amp; category management, shopping basket, order processing, and Stripe payments.",
                Contact = new OpenApiContact
                {
                    Name = "Mahmoud Hussien",
                    Email = "mahmoudhussien@example.com"
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token.\n\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // ─── XML Documentation Comments ──────────────────────────────────
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

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

    private static IServiceCollection AddGlobalExceptionServices(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();
        return services;
    }

    private static IServiceCollection AddFileServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
        );

        services.AddSingleton<IImageManagementService, ImageMangementService>();

        return services;
    }

    private static IServiceCollection AddAuthSystem(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.EffectiveKey) &&
                !string.IsNullOrWhiteSpace(options.Issuer) &&
                !string.IsNullOrWhiteSpace(options.Audience) &&
                options.ExpiryMinutes > 0,
                "Jwt configuration is invalid. Set Jwt:Issuer, Jwt:Audience, Jwt:ExpiryMinutes, and provide Jwt:Key or the Jwt__Key environment variable.")
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.EffectiveKey)),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience
            };
        });

        services.AddAuthorization();

        return services;
    }
}
