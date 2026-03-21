using E_commerce.Core.Common;

namespace E_commerce.Core.Errors;

public static class BasketErrors
{
    public static readonly Error NotFound = new(
        "Basket.NotFound", 
        "The basket with the specified ID was not found.", 
        404);

    public static readonly Error UpdateFailed = new(
        "Basket.UpdateFailed", 
        "Something went wrong while updating the basket.", 
        500);

    public static readonly Error DeletionFailed = new(
        "Basket.DeletionFailed", 
        "Something went wrong while deleting the basket.", 
        500);
}
