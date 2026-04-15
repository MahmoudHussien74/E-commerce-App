using E_commerce.Core.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_commerce.Infrastructure.Service;

public class StripeService(IOptions<PaymentSettings> options) : IStripeService
{
    private readonly PaymentSettings _options = options.Value;

    public async Task<PaymentIntent> CreatePaymentIntentAsync(PaymentIntentCreateOptions createOptions)
    {
        StripeConfiguration.ApiKey = _options.SecretKey;
        var service = new PaymentIntentService();
        return await service.CreateAsync(createOptions);
    }

    public async Task<PaymentIntent> UpdatePaymentIntentAsync(string id, PaymentIntentUpdateOptions updateOptions)
    {
        StripeConfiguration.ApiKey = _options.SecretKey;
        var service = new PaymentIntentService();
        return await service.UpdateAsync(id, updateOptions);
    }

    public Event ConstructEvent(string json, string signature, string webhookSecret)
    {
        return EventUtility.ConstructEvent(json, signature, webhookSecret);
    }
}
