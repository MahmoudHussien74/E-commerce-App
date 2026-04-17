using Microsoft.AspNetCore.Authorization;

namespace E_commerce.Infrastructure.Authentication.Permissions
{
    public class PermissionsRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
