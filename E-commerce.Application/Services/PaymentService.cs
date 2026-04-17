using E_commerce.Application.Abstractions.Persistence;
using E_commerce.Application.Abstractions.Services;
using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Basket;
using E_commerce.Application.Errors;
using E_commerce.Core.Entities.Order;

namespace E_commerce.Application.Services;

internal sealed class PaymentService(
    IUnitOfWork unitOfWork,
    IStripeGateway stripeGateway,
    IInventoryService inventoryService) : IPaymentService
{
    public async Task<Result<CustomerBasketResponse>> CreateOrUpdatePaymentAsync(string buyerId, int? deliveryMethodId, CancellationToken cancellationToken = default)
    {
        var basketResult = await unitOfWork.CustomerBasketRepository.GetBasketAsync(buyerId, cancellationToken);
        if (basketResult.IsFailure)
        {
            return Result.Failure<CustomerBasketResponse>(PaymentErrors.BasketNotFound);
        }

        var basket = basketResult.Value;
        var shippingPrice = basket.ShippingPrice;

        foreach (var item in basket.BasketItems)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(item.Id, cancellationToken);
            if (product is not null && item.Price != product.NewPrice)
            {
                item.Price = product.NewPrice;
            }
        }

        if (deliveryMethodId.HasValue)
        {
            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId.Value, cancellationToken);
            if (deliveryMethod is null)
            {
                return Result.Failure<CustomerBasketResponse>(PaymentErrors.DeliveryMethodNotFound);
            }

            shippingPrice = deliveryMethod.Price;
            basket.DeliveryMethodId = deliveryMethodId.Value;
            basket.ShippingPrice = shippingPrice;
        }

        var amount = (long)(basket.BasketItems.Sum(x => x.Qunatity * x.Price * 100m) + shippingPrice * 100m);

        if (string.IsNullOrWhiteSpace(basket.PaymentIntentId))
        {
            var paymentIntent = await stripeGateway.CreatePaymentIntentAsync(amount, "usd", basket.Id, cancellationToken);
            basket.PaymentIntentId = paymentIntent.Id;
            basket.ClientSecret = paymentIntent.ClientSecret;
        }
        else
        {
            await stripeGateway.UpdatePaymentIntentAsync(basket.PaymentIntentId, amount, cancellationToken);
        }

        var result = await unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket, cancellationToken);
        return result.IsFailure
            ? Result.Failure<CustomerBasketResponse>(BasketErrors.UpdateFailed)
            : Result.Success(new CustomerBasketResponse
            {
                Id = result.Value.Id,
                BuyerId = result.Value.BuyerId,
                PaymentIntentId = result.Value.PaymentIntentId,
                ClientSecret = result.Value.ClientSecret,
                DeliveryMethodId = result.Value.DeliveryMethodId,
                ShippingPrice = result.Value.ShippingPrice,
                Items = result.Value.BasketItems.Select(x => new BasketItemResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Quantity = x.Qunatity,
                    Price = x.Price,
                    Category = x.Category
                }).ToList()
            });
    }

    public async Task<Result> ProcessWebhookAsync(string json, string signature, CancellationToken cancellationToken = default)
    {
        var evt = await stripeGateway.ParseWebhookAsync(json, signature, cancellationToken);
        var order = await unitOfWork.Repository<Orders>().GetAsync(x => x.PaymentIntentId == evt.PaymentIntentId, cancellationToken, x => x.OrderItems, x => x.DeliveryMethod);
        if (order is null)
        {
            return Result.Success();
        }

        if (evt.EventType == "payment_intent.succeeded" && order.Status != Status.PaymentReceived)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var stockDeducted = await inventoryService.DeductStockAsync(order.OrderItems, cancellationToken);
                if (!stockDeducted)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure(PaymentErrors.ProviderFailure);
                }

                order.Status = Status.PaymentReceived;
                unitOfWork.Repository<Orders>().Update(order);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        if (evt.EventType == "payment_intent.payment_failed")
        {
            order.Status = Status.PaymentFailed;
            unitOfWork.Repository<Orders>().Update(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
