using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Common;
using E_commerce.Core.Entities.Product;
using E_commerce.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace E_commerce.Infrastructure.Repositories;

internal sealed class ProductRepository(ApplicationDbContext context) : GenericRepository<Product>(context), IProductRepository
{
    private readonly ApplicationDbContext _context = context;


    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
      =>    await _context.Products
           .Include(x => x.Photos)
           .Where(p => ids.Contains(p.Id))
           .ToListAsync(cancellationToken);


    public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Product>()
                    .Where(x => x.Id == id)
                    .Include(x => x.Category)
                    .Include(x => x.Photos)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
    
    public async Task<Product?> GetProductWithPhotosAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<Product>()
                    .Include(x => x.Category)
                    .Include(x => x.Photos)
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

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
        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((filters.PageNumber - 1) * filters.PageSize).Take(filters.PageSize).ToListAsync(cancellationToken);
        return new PaginatedList<Product>(items, filters.PageNumber, count, filters.PageSize);
    }
}
