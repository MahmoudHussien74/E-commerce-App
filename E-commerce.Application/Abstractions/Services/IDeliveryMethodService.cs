using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Order;

namespace E_commerce.Application.Abstractions.Services;

public interface IDeliveryMethodService
{
    Task<Result<DeliveryMethodResponse>> CreateDeliveryMethodAsync(DeliveryMethodRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeliveryMethodResponse>> GetDeliveryMethodByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<DeliveryMethodResponse>>> GetAllDeliveryMethodsAsync(CancellationToken cancellationToken = default);
    Task<Result<DeliveryMethodResponse>> UpdateDeliveryMethodAsync(int id, DeliveryMethodRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteDeliveryMethodAsync(int id, CancellationToken cancellationToken = default);
}
