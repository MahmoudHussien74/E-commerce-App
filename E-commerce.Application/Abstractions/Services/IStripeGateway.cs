namespace E_commerce.Application.Abstractions.Services;

public interface IStripeGateway
{
    Task<PaymentIntentResult> CreatePaymentIntentAsync(long amount, string currency, string basketId, CancellationToken cancellationToken = default);
    Task UpdatePaymentIntentAsync(string paymentIntentId, long amount, CancellationToken cancellationToken = default);
    Task<PaymentWebhookEvent> ParseWebhookAsync(string json, string signature, CancellationToken cancellationToken = default);
}

public sealed record PaymentIntentResult(string Id, string ClientSecret);
public sealed record PaymentWebhookEvent(string EventType, string PaymentIntentId);
