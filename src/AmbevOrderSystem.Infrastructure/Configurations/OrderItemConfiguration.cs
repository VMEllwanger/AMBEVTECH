using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace AmbevOrderSystem.Infrastructure.Configurations
{
    [ExcludeFromCodeCoverage]
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ProductSku)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(e => e.UnitPrice)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.HasOne(e => e.CustomerOrder)
                .WithMany(e => e.Items)
                .HasForeignKey(e => e.CustomerOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}