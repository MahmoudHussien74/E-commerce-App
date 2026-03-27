using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Common;
using E_commerce.Core.Entities.Product;
using E_commerce.Core.Interfaces;
using E_commerce.Infrastructure.Data;
using E_commerce.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_commerce.Infrastructure.Repositories;

public class ProductRepository(ApplicationDbContext context) : GenericRepository<Product>(context), IProductRepository
{
    private readonly ApplicationDbContext _context = context;


    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids)
      =>    await _context.Products
           .Where(p => ids.Contains(p.Id))
           .ToListAsync();


    public async Task<Product> GetProductById(int id, CancellationToken cancellationToken)
        => _context.Set<Product>()
                    .Where(x => x.Id == id)
                    .Include(x => x.Category)
                    .Include(x => x.Photos)
                    .AsNoTracking()
                    .FirstOrDefault()!;
    
    public async Task<Product> GetProductIncludeCategoryAndPhotoAsync(int id, CancellationToken cancellationToken)
        => _context.Set<Product>()
                    .Include(x => x.Category)
                    .Include(x => x.Photos)
                    .FirstOrDefault(x => x.Id == id)!;

    public async Task<PaginatedList<Product>> GetAllProductsAsync(RequestFilter filters, CancellationToken cancellationToken)
    {
        var query = _context.Set<Product>()
            .Include(x => x.Category)
            .Include(x => x.Photos)
            .AsQueryable();


        if (!string.IsNullOrEmpty(filters.SearchValue))
            query = query.Where(x => x.Name.Contains(filters.SearchValue));

        var columnsMap = new Dictionary<string, Expression<Func<Product, object>>>
        {
            ["name"] = x => x.Name,
            ["price"] = x => x.NewPrice,
            ["id"] = x => x.Id
        };
        
        query = query.ApplySort(filters.SortColumn, filters.SortDirection, columnsMap);

        return await PaginatedList<Product>.CreateAsync(query, filters.PageNumber, filters.PageSize, cancellationToken);
    }
}
