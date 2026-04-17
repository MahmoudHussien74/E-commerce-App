using E_commerce.Application.Abstractions.Persistence;
using E_commerce.Application.Abstractions.Services;
using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Order;
using E_commerce.Application.Errors;
using E_commerce.Core.Entities.Order;

namespace E_commerce.Application.Services;

internal sealed class DeliveryMethodService(IUnitOfWork unitOfWork) : IDeliveryMethodService
{
    public async Task<Result<DeliveryMethodResponse>> CreateDeliveryMethodAsync(DeliveryMethodRequest request, CancellationToken cancellationToken = default)
    {
        var deliveryMethod = new DeliveryMethod(request.ShortName, request.DeliveryTime, request.Description, request.Price);
        
        unitOfWork.Repository<DeliveryMethod>().Add(deliveryMethod);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(deliveryMethod));
    }

    public async Task<Result<DeliveryMethodResponse>> GetDeliveryMethodByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(id, cancellationToken);
        if (deliveryMethod is null)
        {
            return Result.Failure<DeliveryMethodResponse>(DeliveryMethodErrors.NotFound);
        }

        return Result.Success(Map(deliveryMethod));
    }

    public async Task<Result<IReadOnlyList<DeliveryMethodResponse>>> GetAllDeliveryMethodsAsync(CancellationToken cancellationToken = default)
    {
        var methods = await unitOfWork.Repository<DeliveryMethod>().GetAllAsync(cancellationToken);
        return Result.Success<IReadOnlyList<DeliveryMethodResponse>>(methods.Select(Map).ToList());
    }

    public async Task<Result<DeliveryMethodResponse>> UpdateDeliveryMethodAsync(int id, DeliveryMethodRequest request, CancellationToken cancellationToken = default)
    {
        var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(id, cancellationToken);
        if (deliveryMethod is null)
        {
            return Result.Failure<DeliveryMethodResponse>(DeliveryMethodErrors.NotFound);
        }

        deliveryMethod.ShortName = request.ShortName;
        deliveryMethod.DeliveryTime = request.DeliveryTime;
        deliveryMethod.Description = request.Description;
        deliveryMethod.Price = request.Price;

        unitOfWork.Repository<DeliveryMethod>().Update(deliveryMethod);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(deliveryMethod));
    }

    public async Task<Result> DeleteDeliveryMethodAsync(int id, CancellationToken cancellationToken = default)
    {
        var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(id, cancellationToken);
        if (deliveryMethod is null)
        {
            return Result.Failure(DeliveryMethodErrors.NotFound);
        }

        await unitOfWork.Repository<DeliveryMethod>().DeleteAsync(deliveryMethod, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static DeliveryMethodResponse Map(DeliveryMethod method)
        => new()
        {
            Id = method.Id,
            ShortName = method.ShortName,
            Description = method.Description,
            DeliveryTime = method.DeliveryTime,
            Price = method.Price
        };
}
