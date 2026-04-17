using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Infrastructure.Data.Configurations;

public partial class UserConfigurations
{
    public partial class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {

            var permissions = Permissions.GetAllPermissions();
            var adminClaims = new List<IdentityRoleClaim<string>>();

            for (int i = 0; i < permissions.Count; i++)
            {
                adminClaims.Add(new IdentityRoleClaim<string>()
                {
                    Id = i + 1,
                    RoleId = DefultRoles.AdminRoleId,
                    ClaimType = Permissions.Type,
                    ClaimValue = permissions[i]

                });
            }
            builder.HasData(adminClaims);
        }
