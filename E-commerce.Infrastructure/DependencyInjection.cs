namespace E_commerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories()
                .AddPersistence(configuration)
                .AddCaching(configuration)
                .AddPaymentConfigurations(configuration)
                .AddIdentityConfiguration()
                .AddAuthenticationServices();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<ICustomerBasketRepository, CustomerBasketRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IStripeGateway, StripeService>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("EcommerceDatabase"));
        });

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var connectionString = configuration.GetConnectionString("redis")
                ?? "localhost:6379,abortConnect=false";

            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 3000;
            options.ReconnectRetryPolicy = new ExponentialRetry(1000);

            var multiplexer = ConnectionMultiplexer.Connect(options);

            if (!multiplexer.IsConnected)
            {
                var logger = serviceProvider
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Redis");

                logger.LogWarning(
                    "Redis is not reachable at '{Endpoint}'. " +
                    "Basket and caching features will be unavailable until Redis is restored.",
                    connectionString);
            }

            return multiplexer;
        });

        return services;
    }

    private static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddPaymentConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PaymentSettings>(configuration.GetSection("StripeSettings"));

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, AuthService>();
        services.AddScoped<ITokenService, JwtProvider>();
        return services;
    }
}
