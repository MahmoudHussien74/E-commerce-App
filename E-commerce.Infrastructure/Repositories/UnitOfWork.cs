using E_commerce.Core.Interfaces;
using E_commerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        CategoryRepository = new CategoryRepository(context);
        ProductRepository = new ProductRepository(context);
        PhotoRepository = new PhotoRepository(context);
    }
    public ICategoryRepository CategoryRepository { get; }

    public IProductRepository ProductRepository { get; }
    public IPhotoRepository PhotoRepository { get; }


}
