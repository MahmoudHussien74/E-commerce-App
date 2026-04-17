using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Infrastructure.Data.Configurations;

public class IdentityRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
            new ApplicationRole
            {
                Id = DefultRoles.AdminRoleId,
                Name = DefultRoles.Admin,
                NormalizedName = DefultRoles.Admin.ToUpperInvariant(),
                ConcurrencyStamp = DefultRoles.AdminRoleConcurrencyStamp
            },
            new ApplicationRole
            {
                Id = DefultRoles.CustomerRoleId,
                Name = DefultRoles.Customer,
                NormalizedName = DefultRoles.Customer.ToUpperInvariant(),
                ConcurrencyStamp = DefultRoles.CustomerRoleConcurrencyStamp,
                IsDefault = true
            });
    }
}
