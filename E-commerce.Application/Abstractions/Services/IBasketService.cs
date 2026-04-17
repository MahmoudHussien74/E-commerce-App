using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Basket;

namespace E_commerce.Application.Abstractions.Services;

public interface IBasketService
{
    Task<Result<CustomerBasketResponse>> GetBasketAsync(string buyerId, CancellationToken cancellationToken = default);
    Task<Result<CustomerBasketResponse>> UpdateBasketAsync(string buyerId, BasketUpdateRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteBasketAsync(string buyerId, CancellationToken cancellationToken = default);
    Task<Result<CustomerBasketResponse>> DeleteItemAsync(string buyerId, int itemId, CancellationToken cancellationToken = default);
}
