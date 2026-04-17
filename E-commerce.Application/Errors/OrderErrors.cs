using E_commerce.Application.Common;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Errors;

public static class OrderErrors
{
    public static readonly Error DeliveryMethodNotFound = new("Order.DeliveryMethodNotFound", "The delivery method was not found.", StatusCodes.Status400BadRequest);
    public static readonly Error EmptyBasket = new("Order.EmptyBasket", "The basket does not contain any valid items.", StatusCodes.Status400BadRequest);
    public static readonly Error NotFound = new("Order.NotFound", "The order was not found.", StatusCodes.Status404NotFound);
}
