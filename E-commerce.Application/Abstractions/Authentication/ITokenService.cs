namespace E_commerce.Application.Abstractions.Authentication;

public interface ITokenService
{
    AccessTokenResult CreateAccessToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
    TokenPrincipal? ReadPrincipal(string accessToken, bool validateLifetime);
}

public sealed record AccessTokenResult(string Token, string JwtId, DateTime ExpiresAtUtc);
public sealed record TokenPrincipal(string UserId, string Email, string JwtId);
