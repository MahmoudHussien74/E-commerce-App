using E_commerce.Core.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_commerce.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Orders>
{
    public void Configure(EntityTypeBuilder<Orders> builder)
    {
        builder.OwnsOne(x => x.ShippingAddress, n => { n.WithOwner(); });

        builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Status).HasConversion(
            o => o.ToString(),
            o => (Status)Enum.Parse(typeof(Status), o)
        );

        builder.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
    }
}
