using E_commerce.Core.Entities.Order;

namespace E_commerce.Core.Interfaces;

public interface IInventoryService
{
    Task<bool> DeductStockAsync(IEnumerable<OrderItem> items);
}
