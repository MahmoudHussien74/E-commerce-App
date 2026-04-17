using E_commerce.Application.Common;
using E_commerce.Core.Entities;

namespace E_commerce.Application.Abstractions.Persistence;

public interface ICustomerBasketRepository
{
    Task<Result<CustomerBasket>> GetBasketAsync(string buyerId, CancellationToken cancellationToken = default);
    Task<Result<CustomerBasket>> UpdateBasketAsync(CustomerBasket basket, CancellationToken cancellationToken = default);
    Task<Result> DeleteBasketAsync(string buyerId, CancellationToken cancellationToken = default);
}
