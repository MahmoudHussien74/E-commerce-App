using E_commerce.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Infrastructure.Data.Configurations;

public class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();
        
        builder.Property(x => x.Name)
                .IsRequired();
        builder.Property(x => x.Description)
                .IsRequired();

        builder.Property(x => x.NewPrice)
               .HasColumnType("decimal(18,2)");

        builder.HasData(new Product
        {
            Id = 1,
            Name = "Test",
            Description = "Test",
            CategoryId = 1,
            NewPrice=12
        });

    }
}
