using E_commerce.Core.Common;

namespace E_commerce.Core.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound = new(
        "Category.NotFound", 
        "The category with the specified ID was not found.", 
        404);
}
