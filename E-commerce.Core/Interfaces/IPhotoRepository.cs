namespace E_commerce.Core.Interfaces;

public interface IPhotoRepository : IGenericRepository<Photo> 
{
   Task<List<Photo>> GetPhotoByProductId(int id);
}
