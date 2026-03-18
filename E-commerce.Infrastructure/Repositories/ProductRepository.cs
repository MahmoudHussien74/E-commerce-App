using E_commerce.Core.Entities.Product;
using E_commerce.Core.Interfaces;
using E_commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Infrastructure.Repositories;

public class ProductRepository(ApplicationDbContext context) : GenericRepository<Product>(context), IProductRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Product> GetProductById(int id, CancellationToken cancellationToken)
        => _context.Set<Product>()
                    .Where(x => x.Id == id)
                    .Include(x => x.Category)
                    .Include(x => x.Photos)
                    .AsNoTracking()
                    .FirstOrDefault()!;
    

      
    
}
