using Microsoft.AspNetCore.Authorization;

namespace E_commerce.Infrastructure.Authentication.Permissions
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission) : base(permission) { }
    }
}
