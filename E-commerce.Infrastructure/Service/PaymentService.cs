using Microsoft.Extensions.Logging;
using Stripe;
using E_commerce.Core.Entities.Order;
using Microsoft.Extensions.Options;
using E_commerce.Core.Interfaces;
using E_commerce.Core.Entities;
using E_commerce.Core.Contracts.Basket;
using AutoMapper;

namespace E_commerce.Infrastructure.Service;

public class PaymentService(
    IUnitOfWork unitOfWork, 
    IStripeService stripeService,
    IInventoryService inventoryService,
    IMapper mapper,
    IOptions<PaymentSettings> options,
    ILogger<PaymentService> logger) : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IStripeService _stripeService = stripeService;
    private readonly IInventoryService _inventoryService = inventoryService;
    private readonly IMapper _mapper = mapper;
    private readonly PaymentSettings _options = options.Value;
    private readonly ILogger<PaymentService> _logger = logger;

    public async Task<Result<CustomerBasketResponse>> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethodId)
    {
        _logger.LogInformation("Creating or updating PaymentIntent for basket: {BasketId}", basketId);

        var basketResult = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(basketId);
        if (!basketResult.IsSuccess)
        {
             _logger.LogWarning("Basket {BasketId} not found", basketId);
             return Result.Failure<CustomerBasketResponse>(new Error("Payment.BasketNotFound", "The requested basket was not found.", 404));
        }

        var basket = basketResult.Value;
        var shippingPrice = 0m;

        // Pricing logic: Sync prices from database
        foreach (var item in basket.basketItems)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.Id);
            if (product != null && item.Price != product.NewPrice)
            {
                item.Price = product.NewPrice;
            }
        }

        if (deliveryMethodId.HasValue)
        {
            var delivery = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId.Value);
            if (delivery == null)
            {
                 return Result.Failure<CustomerBasketResponse>(new Error("Payment.DeliveryMethodNotFound", "Selected delivery method is invalid.", 400));
            }

            shippingPrice = delivery.Price;
            basket.DeliveryMethodId = deliveryMethodId;
            basket.ShippingPrice = shippingPrice;
        }

        var amount = (long)(basket.basketItems.Sum(i => i.Qunatity * (i.Price * 100)) + (shippingPrice * 100));

        try
        {
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var createOptions = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"],
                    Metadata = new Dictionary<string, string> { { "basketId", basketId } }
                };
                var intent = await _stripeService.CreatePaymentIntentAsync(createOptions);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var updateOptions = new PaymentIntentUpdateOptions { Amount = amount };
                await _stripeService.UpdatePaymentIntentAsync(basket.PaymentIntentId, updateOptions);
            }
        }
        catch (StripeException ex)
        {
            return Result.Failure<CustomerBasketResponse>(new Error("Payment.StripeError", ex.Message, 400));
        }

        await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);
        
        var response = _mapper.Map<CustomerBasketResponse>(basket);
        return Result.Success(response);
    }

    public async Task<Result> ProcessWebhookAsync(string json, string signature)
    {
        try
        {
            var stripeEvent = _stripeService.ConstructEvent(json, signature, _options.WebhookSecret);
            
            if (stripeEvent.Data.Object is PaymentIntent paymentIntent)
            {
                switch (stripeEvent.Type)
                {
                    case EventTypes.PaymentIntentSucceeded:
                        await UpdateOrderPaymentStatus(paymentIntent.Id, Status.PaymentReceived);
                        break;
                    case EventTypes.PaymentIntentPaymentFailed:
                        await UpdateOrderPaymentStatus(paymentIntent.Id, Status.PaymentFailed);
                        break;
                }
            }

            return Result.Success();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe signature verification failed");
            return Result.Failure(new Error("Payment.WebhookError", "Webhook verification failed.", 401));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during webhook processing");
            // Securely masking the exact exception details
            return Result.Failure(new Error("Payment.GeneralError", "A server error occurred while processing the payment update.", 500));
        }
    }

    private async Task UpdateOrderPaymentStatus(string paymentIntentId, Status status)
    {
        var order = await _unitOfWork.Repository<Orders>().GetAsync(
            x => x.PaymentIntentId == paymentIntentId, 
            default, 
            x => x.OrderItems);

        if (order == null || order.Status == status) return;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (status == Status.PaymentReceived)
            {
                var stockDeducted = await _inventoryService.DeductStockAsync(order.OrderItems);
                if (!stockDeducted)
                {
                    _logger.LogError("Failed to deduct stock for order {OrderId}. Aborting status update.", order.Id);
                    await _unitOfWork.RollbackAsync();
                    return;
                }
            }

            order.Status = status;
            await _unitOfWork.Repository<Orders>().UpdateAsync(order);
            await _unitOfWork.CompleteAsync();
            
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Order {OrderId} successfully updated to {Status} with Transactional Integrity.", order.Id, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical failure during Order Status Transaction for {OrderId}", order.Id);
            await _unitOfWork.RollbackAsync();
            throw; // Re-throw to be caught by the calling ProcessWebhookAsync catch block
        }
    }
}
