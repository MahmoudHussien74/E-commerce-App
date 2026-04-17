using Microsoft.AspNetCore.Identity;

namespace E_commerce.Core.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsDefault { get; set; }
    }
}