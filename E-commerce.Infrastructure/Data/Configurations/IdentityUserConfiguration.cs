using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Infrastructure.Data.Configurations;

public class IdentityUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.OwnsMany(x => x.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        builder.Property(x => x.FirstName)
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasMaxLength(100);

        builder.HasData(new User
        {
            Id = DefultUsers.AdminId,
            FirstName = "Ecommerce",
            LastName = "Admin",
            UserName = DefultUsers.AdminEmail,
            NormalizedUserName = DefultUsers.AdminEmail.ToUpperInvariant(),
            Email = DefultUsers.AdminEmail,
            NormalizedEmail = DefultUsers.AdminEmail.ToUpperInvariant(),
            SecurityStamp = DefultUsers.AdminSecurityStamp,
            ConcurrencyStamp = DefultUsers.AdminConcurrencyStamp,
            EmailConfirmed = true,
            PasswordHash = DefultUsers.AdminPassword
        });
    }
}
