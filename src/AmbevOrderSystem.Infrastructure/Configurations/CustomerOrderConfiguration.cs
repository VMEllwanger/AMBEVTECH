using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmbevOrderSystem.Infrastructure.Configurations
{
    public class CustomerOrderConfiguration : IEntityTypeConfiguration<CustomerOrder>
    {
        public void Configure(EntityTypeBuilder<CustomerOrder> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CustomerIdentification)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.AmbevOrderNumber)
                .HasMaxLength(50);

            builder.HasOne(e => e.Reseller)
                .WithMany(e => e.Orders)
                .HasForeignKey(e => e.ResellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Items)
                .WithOne(e => e.CustomerOrder)
                .HasForeignKey(e => e.CustomerOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}