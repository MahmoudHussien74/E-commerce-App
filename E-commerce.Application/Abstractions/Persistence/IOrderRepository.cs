using E_commerce.Core.Entities.Order;

namespace E_commerce.Application.Abstractions.Persistence;

public interface IOrderRepository : IGenericRepository<Orders>
{
    Task<IReadOnlyList<Orders>> GetAllOrdersForUserAsync(string buyerId, CancellationToken cancellationToken = default);
    Task<Orders?> GetOrderForUserAsync(string buyerId, int id, CancellationToken cancellationToken = default);
    Task<Orders?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
}
