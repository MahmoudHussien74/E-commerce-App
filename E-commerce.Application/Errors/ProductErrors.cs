using E_commerce.Application.Common;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new("Product.NotFound", "The product was not found.", StatusCodes.Status404NotFound);
    public static readonly Error AdditionFailed = new("Product.AdditionFailed", "The product could not be created.", StatusCodes.Status500InternalServerError);
    public static readonly Error UpdateFailed = new("Product.UpdateFailed", "The product could not be updated.", StatusCodes.Status500InternalServerError);
}
