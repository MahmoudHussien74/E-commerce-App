using Microsoft.AspNetCore.Identity;

namespace E_commerce.Infrastructure.Data;

public class ApplicationRole : IdentityRole
{
    public bool IsDefault { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
