using E_commerce.Application.Common;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Errors;

public static class PaymentErrors
{
    public static readonly Error BasketNotFound = new("Payment.BasketNotFound", "The basket was not found.", StatusCodes.Status404NotFound);
    public static readonly Error DeliveryMethodNotFound = new("Payment.DeliveryMethodNotFound", "The delivery method was not found.", StatusCodes.Status400BadRequest);
    public static readonly Error ProviderFailure = new("Payment.ProviderFailure", "The payment provider rejected the request.", StatusCodes.Status400BadRequest);
}
