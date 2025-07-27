using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AmbevOrderSystem.Infrastructure.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(e => e.Reseller)
                .WithMany(e => e.Contacts)
                .HasForeignKey(e => e.ResellerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}