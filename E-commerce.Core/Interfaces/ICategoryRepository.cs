using E_commerce.Core.Entities.Product;

namespace E_commerce.Core.Interfaces;

public interface ICategoryRepository:IGenericRepository<Category> 
{

}
public interface IPhotoRepository : IGenericRepository<Photo> 
{
   Task<List<Photo>> GetPhotoByProductId(int id);
}
