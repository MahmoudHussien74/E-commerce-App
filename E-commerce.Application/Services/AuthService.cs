using E_commerce.Application.Abstractions.Authentication;
using E_commerce.Application.Abstractions.Persistence;
using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Authentication;
using E_commerce.Application.Errors;
using E_commerce.Core.Entities.Identity;

namespace E_commerce.Application.Services;

internal sealed class AuthService(
    IIdentityService identityService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : IAuthService
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await identityService.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result.Failure<RegisterResponse>(UserErrors.DuplicatedEmail);
        }

        var createResult = await identityService.CreateUserAsync(request, cancellationToken);
        if (createResult.IsFailure)
        {
            return Result.Failure<RegisterResponse>(createResult.Error);
        }

        var user = createResult.Value;
        var displayName = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));

        return Result.Success(new RegisterResponse(user.Id, displayName, user.Email));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var authenticated = await identityService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
        if (authenticated is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        if (authenticated.User.IsDisabled)
        {
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);
        }

        return await IssueTokensAsync(authenticated.User, authenticated.Roles, authenticated.Permissions, cancellationToken);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var principal = tokenService.ReadPrincipal(request.AccessToken, validateLifetime: false);
        if (principal is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
        }

        var providedHash = tokenService.HashRefreshToken(request.RefreshToken);
        var storedToken = await unitOfWork.RefreshTokens.GetByHashAsync(providedHash, cancellationToken);
        if (storedToken is null || storedToken.UserId != principal.UserId || storedToken.JwtId != principal.JwtId)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
        }

        if (storedToken.RevokedOnUtc is not null)
        {
            await RevokeTokenChainAsync(storedToken.ReplacedByTokenHash, "Refresh token reuse detected.", cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<AuthResponse>(UserErrors.RefreshTokenReuseDetected);
        }

        if (storedToken.IsExpired)
        {
            storedToken.RevokedOnUtc = DateTime.UtcNow;
            storedToken.ReasonRevoked = "Expired";
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
        }

        var user = await identityService.GetByIdAsync(principal.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);
        }

        var roles = await identityService.GetRolesAsync(user.Id, cancellationToken);
        var permissions = await identityService.GetPermissionsAsync(user.Id, cancellationToken);
        var response = await IssueTokensAsync(user, roles, permissions, cancellationToken);
        if (response.IsFailure)
        {
            return response;
        }

        storedToken.RevokedOnUtc = DateTime.UtcNow;
        storedToken.ReplacedByTokenHash = tokenService.HashRefreshToken(response.Value.RefreshToken);
        storedToken.ReasonRevoked = "Rotated";
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }

    private async Task<Result<AuthResponse>> IssueTokensAsync(
        ApplicationUser user,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions,
        CancellationToken cancellationToken)
    {
        var accessToken = tokenService.CreateAccessToken(user, roles, permissions);
        var rawRefreshToken = tokenService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            JwtId = accessToken.JwtId,
            TokenHash = tokenService.HashRefreshToken(rawRefreshToken),
            ExpiresOnUtc = DateTime.UtcNow.Add(RefreshTokenLifetime)
        };

        await unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var displayName = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));

        return Result.Success(new AuthResponse(
            user.Id,
            displayName,
            user.Email,
            accessToken.Token,
            rawRefreshToken,
            accessToken.ExpiresAtUtc,
            refreshToken.ExpiresOnUtc,
            roles,
            permissions));
    }

    private async Task RevokeTokenChainAsync(string? replacedByTokenHash, string reason, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(replacedByTokenHash))
        {
            return;
        }

        var descendants = await unitOfWork.RefreshTokens.GetActiveDescendantsAsync(replacedByTokenHash, cancellationToken);
        foreach (var token in descendants.Where(x => x.IsActive))
        {
            token.RevokedOnUtc = DateTime.UtcNow;
            token.ReasonRevoked = reason;
        }
    }
}
