using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace AmbevOrderSystem.Infrastructure.Configurations
{
    [ExcludeFromCodeCoverage]
    public class DeliveryAddressConfiguration : IEntityTypeConfiguration<DeliveryAddress>
    {
        public void Configure(EntityTypeBuilder<DeliveryAddress> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Street)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Number)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Complement)
                .HasMaxLength(100);

            builder.Property(e => e.Neighborhood)
                .HasMaxLength(100);

            builder.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.State)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.ZipCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(e => e.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(e => e.Reseller)
                .WithMany(e => e.DeliveryAddresses)
                .HasForeignKey(e => e.ResellerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}