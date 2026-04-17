namespace E_commerce.Infrastructure.Repositories;

internal sealed class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        => context.RefreshTokens.AddAsync(refreshToken, cancellationToken).AsTask();

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public async Task<IReadOnlyList<RefreshToken>> GetActiveDescendantsAsync(string replacedByTokenHash, CancellationToken cancellationToken = default)
        => await context.RefreshTokens
            .Where(x => x.TokenHash == replacedByTokenHash || x.ReplacedByTokenHash == replacedByTokenHash)
            .ToListAsync(cancellationToken);
}
