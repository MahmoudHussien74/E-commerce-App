using E_commerce.Application.Common;
using E_commerce.Application.Contracts.Authentication;

namespace E_commerce.Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<ApplicationUser>> CreateUserAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthenticatedIdentity?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> GetRolesAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken = default);
}

public sealed record ApplicationUser(string Id, string Email, string FirstName, string LastName, bool IsDisabled);
public sealed record AuthenticatedIdentity(ApplicationUser User, IReadOnlyCollection<string> Roles, IReadOnlyCollection<string> Permissions);
