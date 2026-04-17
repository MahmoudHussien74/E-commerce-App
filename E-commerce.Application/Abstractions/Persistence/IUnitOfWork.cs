namespace E_commerce.Application.Abstractions.Persistence;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository CategoryRepository { get; }
    IProductRepository ProductRepository { get; }
    IPhotoRepository PhotoRepository { get; }
    ICustomerBasketRepository CustomerBasketRepository { get; }
    IOrderRepository OrderRepository { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
