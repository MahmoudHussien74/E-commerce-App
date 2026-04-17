namespace E_commerce.Application.Contracts.Authentication;

public record AuthResponse(
    string UserId,
    string Name,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
