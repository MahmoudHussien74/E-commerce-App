namespace E_commerce.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public UnitOfWork(ApplicationDbContext context, UserManager<User> userManager, IConnectionMultiplexer connectionMultiplexer)
    {
        _context = context;
        _userManager = userManager;
        _connectionMultiplexer = connectionMultiplexer;
        CategoryRepository = new CategoryRepository(context);
        ProductRepository = new ProductRepository(context);
        PhotoRepository = new PhotoRepository(context);
        CustomerBasketRepository = new CustomerBasketRepository(connectionMultiplexer);
        AuthService = new AuthService(userManager);
    }
    public ICategoryRepository CategoryRepository { get; }

    public IProductRepository ProductRepository { get; }
    public IPhotoRepository PhotoRepository { get; }

    public ICustomerBasketRepository CustomerBasketRepository { get; }

    public IAuthService AuthService { get; }
}
