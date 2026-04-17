using E_commerce.Application.Abstractions.Persistence;
using E_commerce.Application.Abstractions.Services;
using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Basket;
using E_commerce.Application.Errors;
using E_commerce.Core.Entities;

namespace E_commerce.Application.Services;

internal sealed class BasketService(IUnitOfWork unitOfWork) : IBasketService
{
    public async Task<Result<CustomerBasketResponse>> GetBasketAsync(string buyerId, CancellationToken cancellationToken = default)
    {
        var result = await unitOfWork.CustomerBasketRepository.GetBasketAsync(buyerId, cancellationToken);
        return result.IsFailure
            ? Result.Failure<CustomerBasketResponse>(result.Error)
            : Result.Success(Map(result.Value));
    }

    public async Task<Result<CustomerBasketResponse>> UpdateBasketAsync(string buyerId, BasketUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var basket = new CustomerBasket(buyerId)
        {
            DeliveryMethodId = request.DeliveryMethodId,
            ShippingPrice = request.ShippingPrice,
            BasketItems = request.Items.Select(x => new BasketItem
            {
                Id = x.Id,
                Name = x.Name,
                Qunatity = x.Quantity,
                Price = x.Price,
                Category = x.Category
            }).ToList()
        };

        var result = await unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket, cancellationToken);
        return result.IsFailure
            ? Result.Failure<CustomerBasketResponse>(BasketErrors.UpdateFailed)
            : Result.Success(Map(result.Value));
    }

    public Task<Result> DeleteBasketAsync(string buyerId, CancellationToken cancellationToken = default)
        => unitOfWork.CustomerBasketRepository.DeleteBasketAsync(buyerId, cancellationToken);

    public async Task<Result<CustomerBasketResponse>> DeleteItemAsync(string buyerId, int itemId, CancellationToken cancellationToken = default)
    {
        var basketResult = await unitOfWork.CustomerBasketRepository.GetBasketAsync(buyerId, cancellationToken);
        if (basketResult.IsFailure) return Result.Failure<CustomerBasketResponse>(basketResult.Error);

        var basket = basketResult.Value;
        var itemToRemove = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);
        if (itemToRemove is null) return Result.Failure<CustomerBasketResponse>(BasketErrors.ItemNotFound);

        basket.BasketItems.Remove(itemToRemove);
        
        var updateResult = await unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket, cancellationToken);
        return updateResult.IsFailure
            ? Result.Failure<CustomerBasketResponse>(BasketErrors.UpdateFailed)
            : Result.Success(Map(updateResult.Value));
    }

    private static CustomerBasketResponse Map(CustomerBasket basket)
        => new()
        {
            Id = basket.Id,
            BuyerId = basket.BuyerId,
            PaymentIntentId = basket.PaymentIntentId,
            ClientSecret = basket.ClientSecret,
            DeliveryMethodId = basket.DeliveryMethodId,
            ShippingPrice = basket.ShippingPrice,
            Items = basket.BasketItems.Select(x => new BasketItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                Quantity = x.Qunatity,
                Price = x.Price,
                Category = x.Category
            }).ToList()
        };
}
