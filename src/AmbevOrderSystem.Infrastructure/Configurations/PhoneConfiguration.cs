using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmbevOrderSystem.Infrastructure.Configurations
{
    public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
    {
        public void Configure(EntityTypeBuilder<Phone> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Number)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(e => e.Reseller)
                .WithMany(e => e.Phones)
                .HasForeignKey(e => e.ResellerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}