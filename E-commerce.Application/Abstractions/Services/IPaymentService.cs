using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Basket;

namespace E_commerce.Application.Abstractions.Services;

public interface IPaymentService
{
    Task<Result<CustomerBasketResponse>> CreateOrUpdatePaymentAsync(string buyerId, int? deliveryMethodId, CancellationToken cancellationToken = default);
    Task<Result> ProcessWebhookAsync(string json, string signature, CancellationToken cancellationToken = default);
}
