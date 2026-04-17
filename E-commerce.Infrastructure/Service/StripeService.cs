using Microsoft.Extensions.Options;
using Stripe;

namespace E_commerce.Infrastructure.Service;

public class StripeService(IOptions<PaymentSettings> options) : IStripeGateway
{
    private readonly PaymentSettings _options = options.Value;

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(long amount, string currency, string basketId, CancellationToken cancellationToken = default)
    {
        StripeConfiguration.ApiKey = _options.SecretKey;
        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = currency,
            PaymentMethodTypes = ["card"],
            Metadata = new Dictionary<string, string> { ["basketId"] = basketId }
        }, cancellationToken: cancellationToken);

        return new PaymentIntentResult(intent.Id, intent.ClientSecret);
    }

    public async Task UpdatePaymentIntentAsync(string id, long amount, CancellationToken cancellationToken = default)
    {
        StripeConfiguration.ApiKey = _options.SecretKey;
        var service = new PaymentIntentService();
        await service.UpdateAsync(id, new PaymentIntentUpdateOptions { Amount = amount }, cancellationToken: cancellationToken);
    }

    public Task<PaymentWebhookEvent> ParseWebhookAsync(string json, string signature, CancellationToken cancellationToken = default)
    {
        var stripeEvent = EventUtility.ConstructEvent(json, signature, _options.WebhookSecret);
        var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
        return Task.FromResult(new PaymentWebhookEvent(stripeEvent.Type, paymentIntent.Id));
    }
}
