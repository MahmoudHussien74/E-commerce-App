using E_commerce.Application.Common;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Errors;

public static class BasketErrors
{
    public static readonly Error NotFound = new("Basket.NotFound", "The basket was not found.", StatusCodes.Status404NotFound);
    public static readonly Error UpdateFailed = new("Basket.UpdateFailed", "The basket could not be updated.", StatusCodes.Status500InternalServerError);
    public static readonly Error DeletionFailed = new("Basket.DeleteFailed", "The basket could not be deleted.", StatusCodes.Status500InternalServerError);
    public static readonly Error ItemNotFound = new("Basket.ItemNotFound", "The item was not found in the basket.", StatusCodes.Status404NotFound);
}
