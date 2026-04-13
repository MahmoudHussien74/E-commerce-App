namespace E_commerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories()
                .AddPersistence(configuration)
                .AddCaching(configuration)
                .AddPaymentConfigurations(configuration)
                .AddIdentityConfiguration();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPaymentService, PaymentService>();

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
        services.AddSingleton<IConnectionMultiplexer>(i =>
        {
            var config = configuration.GetConnectionString("redis");

            return ConnectionMultiplexer.Connect(config!);
        });

        return services;
    }

    private static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddPaymentConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PaymentSettings>(configuration.GetSection("StripeSettings"));

        return services;
    }
}
