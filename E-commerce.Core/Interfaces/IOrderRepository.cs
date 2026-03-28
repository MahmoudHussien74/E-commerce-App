using E_commerce.Core.Contracts.Order;
using E_commerce.Core.Entities.Order;

namespace E_commerce.Core.Interfaces;

public interface IOrderRepository : IGenericRepository<Orders> 
{
    Task<IReadOnlyList<OrderResponse>> GetAllOrdersForUserProjectedAsync(string buyerEmail);
    Task<OrderResponse> GetOrdersForUserProjectedAsync(string buyerEmail, int id);
    Task<IReadOnlyList<DeliveryMethodResponse>> GetDeliveryMethodsProjectedAsync();
}
