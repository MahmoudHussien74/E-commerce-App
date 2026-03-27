using E_commerce.Core.Entities.Order;
using E_commerce.Core.Contracts.Order;
using E_commerce.Core.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_commerce.Core.Interfaces;

public interface IOrderService
{
    Task<Result<OrderResponse>> CreateOrdersAsync(OrderRequest orderRequest, string BuyerEmail);
    Task<Result<IReadOnlyList<OrderResponse>>> GetAllOrdersForUserAsync(string BuyerEmail);
    Task<Result<OrderResponse>> GetOrderByIdAsync(int Id, string BuyerEmail);
    Task<Result<IReadOnlyList<DeliveryMethodResponse>>> GetDeliveryMethodAsync();
}
