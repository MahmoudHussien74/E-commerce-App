namespace E_commerce.Core.Entities.Identity;

public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresOnUtc { get; set; }
    public DateTime? RevokedOnUtc { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? ReasonRevoked { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;
    public bool IsActive => RevokedOnUtc is null && !IsExpired;
}
