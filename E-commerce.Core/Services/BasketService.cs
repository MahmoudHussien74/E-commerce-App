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
            var result = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(id);

            if (result.IsFailure)
                return Result.Failure<CustomerBasketResponse>(result.Error);

            var response = _mapper.Map<CustomerBasketResponse>(result.Value);

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

            var result = await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);

            if (result.IsFailure)
                return Result.Failure<CustomerBasketResponse>(result.Error);

            var response = _mapper.Map<CustomerBasketResponse>(result.Value);

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
