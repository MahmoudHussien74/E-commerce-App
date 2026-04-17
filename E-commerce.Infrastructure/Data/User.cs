using Microsoft.AspNetCore.Identity;
using E_commerce.Core.Entities.Identity;

namespace E_commerce.Infrastructure.Data;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
