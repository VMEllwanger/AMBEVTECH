using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmbevOrderSystem.Infrastructure.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Data)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.RetryCount)
                .HasDefaultValue(0);

            builder.Property(x => x.MaxRetries)
                .HasDefaultValue(3);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.CorrelationId)
                .HasMaxLength(100);

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.Type);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => x.NextRetryAt);
            builder.HasIndex(x => x.CorrelationId);
        }
    }
}