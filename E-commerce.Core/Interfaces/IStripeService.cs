using Stripe;

namespace E_commerce.Core.Interfaces;

public interface IStripeService
{
    Task<PaymentIntent> CreatePaymentIntentAsync(PaymentIntentCreateOptions options);
    Task<PaymentIntent> UpdatePaymentIntentAsync(string id, PaymentIntentUpdateOptions options);
    Event ConstructEvent(string json, string signature, string webhookSecret);
}
