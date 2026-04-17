using E_commerce.Application.Common;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Application.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Invalid email or password.", StatusCodes.Status401Unauthorized);
    public static readonly Error DuplicatedEmail = new("Auth.DuplicatedEmail", "Another user with the same email already exists.", StatusCodes.Status409Conflict);
    public static readonly Error DisabledUser = new("Auth.DisabledUser", "The user account is disabled.", StatusCodes.Status403Forbidden);
    public static readonly Error InvalidRefreshToken = new("Auth.InvalidRefreshToken", "The refresh token is invalid.", StatusCodes.Status401Unauthorized);
    public static readonly Error RefreshTokenReuseDetected = new("Auth.RefreshTokenReuse", "Refresh token reuse was detected.", StatusCodes.Status401Unauthorized);
    public static readonly Error UserNotFound = new("Auth.UserNotFound", "The user was not found.", StatusCodes.Status404NotFound);
}
