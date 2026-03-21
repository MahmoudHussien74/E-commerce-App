using AutoMapper;
using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Basket;
using E_commerce.Core.Entities;
using E_commerce.Core.Errors;
using E_commerce.Core.Interfaces;

namespace E_commerce.Core.Services;

public class BasketService(IUnitOfWork unitOfWork, IMapper mapper) : IBasketService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<CustomerBasketResponse>> GetBasketAsync(string id)
    {
        try
        {
            var basket = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(id);

            if (basket is null)
                return Result.Failure<CustomerBasketResponse>(BasketErrors.NotFound);

            var response = _mapper.Map<CustomerBasketResponse>(basket);

            return Result.Success(response);
        }
        catch (Exception)
        {
            return Result.Failure<CustomerBasketResponse>(BasketErrors.NotFound);
        }
    }

    public async Task<Result<CustomerBasketResponse>> UpdateBasketAsync(CustomerBasketResponse basketDto)
    {
        try
        {
            var basket = _mapper.Map<CustomerBasket>(basketDto);

            var updatedBasket = await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);

            if (updatedBasket is null)
                return Result.Failure<CustomerBasketResponse>(BasketErrors.UpdateFailed);

            var response = _mapper.Map<CustomerBasketResponse>(updatedBasket);

            return Result.Success(response);
        }
        catch (Exception)
        {
            return Result.Failure<CustomerBasketResponse>(BasketErrors.UpdateFailed);
        }
    }

    public async Task<Result> DeleteBasketAsync(string id)
    {
        try
        {
            var result = await _unitOfWork.CustomerBasketRepository.DeleteBasketAsync(id);

            return result;
        }
        catch (Exception)
        {
            return Result.Failure(BasketErrors.DeletionFailed);
        }
    }
}
