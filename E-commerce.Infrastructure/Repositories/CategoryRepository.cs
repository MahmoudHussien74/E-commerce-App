using E_commerce.Core.Entities.Product;

namespace E_commerce.Infrastructure.Repositories;

internal sealed class CategoryRepository(ApplicationDbContext context) : GenericRepository<Category>(context), ICategoryRepository
{

}
