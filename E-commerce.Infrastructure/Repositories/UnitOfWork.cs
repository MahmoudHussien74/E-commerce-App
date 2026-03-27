using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using E_commerce.Core.Interfaces;
using E_commerce.Core.Entities;
using E_commerce.Infrastructure.Data;
using StackExchange.Redis;

namespace E_commerce.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public UnitOfWork(ApplicationDbContext context, UserManager<User> userManager, IJwtProvider jwtProvider, IConnectionMultiplexer connectionMultiplexer)
    {
        _context = context;
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _connectionMultiplexer = connectionMultiplexer;
        CategoryRepository = new CategoryRepository(context);
        ProductRepository = new ProductRepository(context);
        PhotoRepository = new PhotoRepository(context);
        CustomerBasketRepository = new CustomerBasketRepository(connectionMultiplexer);
        AuthService = new AuthService(userManager,jwtProvider);
    }
    public ICategoryRepository CategoryRepository { get; }

    public IProductRepository ProductRepository { get; }
    public IPhotoRepository PhotoRepository { get; }

    public ICustomerBasketRepository CustomerBasketRepository { get; }

    public IAuthService AuthService { get; }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        return new GenericRepository<T>(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null) await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction?.Dispose();
    }
}
