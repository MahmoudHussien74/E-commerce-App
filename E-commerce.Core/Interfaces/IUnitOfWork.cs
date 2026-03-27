namespace E_commerce.Core.Interfaces;

public interface IUnitOfWork
{
    public ICategoryRepository CategoryRepository { get; }
    public IProductRepository  ProductRepository { get; }
    public IPhotoRepository  PhotoRepository { get; }
    public ICustomerBasketRepository  CustomerBasketRepository { get; }
    public IAuthService AuthService { get; }

    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
