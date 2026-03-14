using E_commerce.Core.Entities.Product;
using E_commerce.Core.Interfaces;
using E_commerce.Infrastructure.Data;

namespace E_commerce.Infrastructure.Repositories;

public class PhotoRepository(ApplicationDbContext context) : GenericRepository<Photo>(context), IPhotoRepository
{

}
