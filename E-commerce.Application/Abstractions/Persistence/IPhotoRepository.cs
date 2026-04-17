using E_commerce.Core.Entities.Product;

namespace E_commerce.Application.Abstractions.Persistence;

public interface IPhotoRepository : IGenericRepository<Photo>
{
    Task<IReadOnlyList<Photo>> GetPhotoByProductId(int productId, CancellationToken cancellationToken = default);
}
