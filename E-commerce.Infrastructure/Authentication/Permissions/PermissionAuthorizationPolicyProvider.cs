using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace E_commerce.Infrastructure.Authentication.Permissions
{
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;
        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);
            if (policy != null)
                return policy;

            var permissionPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionsRequirement(policyName))
                .Build();
            _options.AddPolicy(policyName, permissionPolicy);
            return permissionPolicy;
        }
    }
}
