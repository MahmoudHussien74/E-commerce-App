using E_commerce.Core.Common;
using E_commerce.Core.Entities.Product;
using E_commerce.Core.Interfaces;
using E_commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Infrastructure.Repositories;

public class PhotoRepository(ApplicationDbContext context) : GenericRepository<Photo>(context), IPhotoRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<Photo>> GetPhotoByProductId(int id)
    => await _context.Photos.Where(x => x.ProductId == id).ToListAsync();


}
