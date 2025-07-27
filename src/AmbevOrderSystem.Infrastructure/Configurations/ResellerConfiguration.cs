using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmbevOrderSystem.Infrastructure.Configurations
{
    public class ResellerConfiguration : IEntityTypeConfiguration<Reseller>
    {
        public void Configure(EntityTypeBuilder<Reseller> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.Cnpj).IsUnique();

            builder.Property(e => e.Cnpj)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(e => e.RazaoSocial)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.NomeFantasia)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired();

            builder.HasMany(e => e.Phones)
                .WithOne(e => e.Reseller)
                .HasForeignKey(e => e.ResellerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Contacts)
                .WithOne(e => e.Reseller)
                .HasForeignKey(e => e.ResellerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.DeliveryAddresses)
                .WithOne(e => e.Reseller)
                .HasForeignKey(e => e.ResellerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Orders)
                .WithOne(e => e.Reseller)
                .HasForeignKey(e => e.ResellerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}