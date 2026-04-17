using E_commerce.Application.Common;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound = new("Category.NotFound", "The category was not found.", StatusCodes.Status404NotFound);
}
