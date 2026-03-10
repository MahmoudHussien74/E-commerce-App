using E_commerce.Core.Entites.Product;
using E_commerce.Core.Interfaces;
using E_commerce.Infrastructure.Data;

namespace E_commerce.Infrastructure.Repositries;

public class PhotoRepository(ApplicationDbContext context) : GenericRepository<Photo>(context), IPhotoRepository
{

}
