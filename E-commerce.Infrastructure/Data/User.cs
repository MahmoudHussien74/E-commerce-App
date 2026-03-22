using Microsoft.AspNetCore.Identity;

namespace E_commerce.Infrastructure.Data;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}
