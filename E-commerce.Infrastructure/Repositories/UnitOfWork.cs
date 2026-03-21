using E_commerce.Core.Interfaces;
using E_commerce.Infrastructure.Data;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public UnitOfWork(ApplicationDbContext context,IConnectionMultiplexer connectionMultiplexer)
    {
        _context = context;
        _connectionMultiplexer = connectionMultiplexer;
        CategoryRepository = new CategoryRepository(context);
        ProductRepository = new ProductRepository(context);
        PhotoRepository = new PhotoRepository(context);
        CustomerBasketRepository = new CustomerBasketRepository(connectionMultiplexer);
    }
    public ICategoryRepository CategoryRepository { get; }

    public IProductRepository ProductRepository { get; }
    public IPhotoRepository PhotoRepository { get; }

    public ICustomerBasketRepository CustomerBasketRepository { get; }
}
