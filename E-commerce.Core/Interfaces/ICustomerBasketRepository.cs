using E_commerce.Core.Common;
using E_commerce.Core.Entities;

namespace E_commerce.Core.Interfaces;

public interface ICustomerBasketRepository
{
    Task<Result<CustomerBasket>> GetBasketAsync(string id);
    Task<Result<CustomerBasket>> UpdateBasketAsync(CustomerBasket basket);
    Task<Result> DeleteBasketAsync(string id);
}
