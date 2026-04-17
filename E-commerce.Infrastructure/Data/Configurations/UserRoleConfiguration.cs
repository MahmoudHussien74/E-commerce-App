using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Infrastructure.Data.Configurations;

public partial class UserConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {

            builder.HasData(new IdentityUserRole<string>
            {
                UserId = DefultUsers.AdminId,
                RoleId = DefultRoles.AdminRoleId
            });
        }
