using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Order;
using E_commerce.Core.Entities.Order;

namespace E_commerce.Application.Abstractions.Services;

public interface IOrderService
{
    Task<Result<OrderResponse>> CreateOrderAsync(string buyerId, string buyerEmail, OrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<OrderResponse>>> GetAllOrdersForUserAsync(string buyerId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> GetOrderByIdAsync(int id, string buyerId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> UpdateOrderStatusAsync(int id, Status status, CancellationToken cancellationToken = default);
}
