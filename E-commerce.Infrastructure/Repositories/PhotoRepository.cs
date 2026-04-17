using E_commerce.Core.Entities.Product;

namespace E_commerce.Infrastructure.Repositories;

internal sealed class PhotoRepository(ApplicationDbContext context) : GenericRepository<Photo>(context), IPhotoRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IReadOnlyList<Photo>> GetPhotoByProductId(int id, CancellationToken cancellationToken = default)
        => await _context.Photos.Where(x => x.ProductId == id).ToListAsync(cancellationToken);


}
