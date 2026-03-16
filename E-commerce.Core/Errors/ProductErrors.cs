using E_commerce.Core.Common;

namespace E_commerce.Core.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new(
        "Product.NotFound", 
        "The product with the specified ID was not found.", 
        404);
}
