using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Basket;

namespace E_commerce.Core.Interfaces;

public interface IBasketService
{
    Task<Result<CustomerBasketResponse>> GetBasketAsync(string id);
    Task<Result<CustomerBasketResponse>> UpdateBasketAsync(CustomerBasketResponse basket);
    Task<Result> DeleteBasketAsync(string id);
}
