using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce.Infrastructure.Authentication.Permissions
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
        {
            if (context.User.Identity is not { IsAuthenticated: true })
                return Task.CompletedTask;

            if (context.User.Claims.Any(x => x.Type == PermissionNames.ClaimType && x.Value == requirement.Permission))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
