using ECommerceOS.AuthService.Infrastructure.Persistence.Data.Models;

namespace ECommerceOS.AuthService.Infrastructure.Persistence.Data.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(x => x.MessageId);

        builder.Property(x => x.MessageId).HasColumnName("message_id").ValueGeneratedNever();
        builder.Property(x => x.Type).HasColumnName("type").HasMaxLength(256).IsRequired();
        builder.Property(x => x.IntegrationEvent).HasColumnName("integration_event").IsRequired();
        builder.Property(x => x.Attempts).HasColumnName("attempts");
        builder.Property(x => x.Error).HasColumnName("error");
        builder.Property(x => x.ProcessedOn).HasColumnName("processed_on");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.CreatedAt)
            .HasConversion(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    }
}
