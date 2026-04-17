using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Common;

public record Error(string Code, string Message, int StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, StatusCodes.Status200OK);
}
