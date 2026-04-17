using E_commerce.Core.Entities.Order;

namespace E_commerce.Application.Abstractions.Services;

public interface IInventoryService
{
    Task<bool> DeductStockAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default);
}
