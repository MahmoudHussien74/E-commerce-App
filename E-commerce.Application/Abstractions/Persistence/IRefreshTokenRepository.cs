using E_commerce.Core.Entities.Identity;

namespace E_commerce.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveDescendantsAsync(string replacedByTokenHash, CancellationToken cancellationToken = default);
}
