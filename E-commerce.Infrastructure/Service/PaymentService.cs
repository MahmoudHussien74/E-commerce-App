using Microsoft.Extensions.Options;
using Stripe;

namespace E_commerce.Infrastructure.Service;

public class PaymentService(IUnitOfWork unitOfWork, ApplicationDbContext context, IOptions<PaymentSettings> options) : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ApplicationDbContext _context = context;
    private readonly PaymentSettings _options = options.Value;

    public async Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethod)
    {
        var basketResult = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(basketId);

        if (!basketResult.IsSuccess) return null;

        var basket = basketResult.Value;

        StripeConfiguration.ApiKey = _options.SecretKey;

        var shippingPrice = 0m;

        if (deliveryMethod.HasValue)
        {
            var delivery = await _context.DeliveryMethods
                            .AsNoTracking()
                            .SingleOrDefaultAsync(x => x.Id == deliveryMethod);

            if (delivery is null) return null;

            shippingPrice = delivery.Price;

            foreach (var item in basket.basketItems)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.Id);
                if (product is not null)
                {
                    item.Price = product.NewPrice;
                }
            }
        }

        PaymentIntentService paymentIntentService = new();
        PaymentIntent intent;

        if (string.IsNullOrEmpty(basket.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)basket.basketItems.Sum(i => i.Qunatity * (i.Price * 100)) + (long)shippingPrice * 100,
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };
            intent = await paymentIntentService.CreateAsync(options);
            basket.PaymentIntentId = intent.Id;
            basket.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)basket.basketItems.Sum(i => i.Qunatity * (i.Price * 100)) + (long)shippingPrice * 100
            };
            await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);
        }

        await _unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);

        return basket;
    }
}
