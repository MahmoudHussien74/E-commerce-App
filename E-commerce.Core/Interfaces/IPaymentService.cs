using E_commerce.Core.Contracts.Basket;
using E_commerce.Core.Common;

namespace E_commerce.Core.Interfaces;

public interface IPaymentService
{
    Task<Result<CustomerBasketResponse>> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethod);
    Task<Result> ProcessWebhookAsync(string json, string signature);
}
