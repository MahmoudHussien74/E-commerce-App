using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Infrastructure.Data.Configurations;

public partial class UserConfigurations
{
    public partial class RoleClaimConfiguration
    {
        public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
        {
            public void Configure(EntityTypeBuilder<ApplicationRole> builder)
            {

                var passwordHasher = new PasswordHasher<User>();

                builder.HasData([
                    new ApplicationRole
            {
               Id =DefultRoles.AdminRoleId,
               Name = DefultRoles.Admin,
               NormalizedName = DefultRoles.Admin.ToUpper(),
               ConcurrencyStamp = DefultRoles.AdminRoleConcurrencyStamp,
            },
            new ApplicationRole
            {
                Id = DefultRoles.CustomerRoleId,
                Name = DefultRoles.Customer,
                NormalizedName = DefultRoles.Customer.ToUpper(),
                ConcurrencyStamp = DefultRoles.CustomerRoleConcurrencyStamp,
                IsDefult = true
            }
                ]);
            }
        }
