using E_commerce.Core.Entites.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Infrastructure.Data.Configurations;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(X => X.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasData(new Category
        {
            Id = 1,
            Name = "Test",
            Description = "Test"
        });

    }
}