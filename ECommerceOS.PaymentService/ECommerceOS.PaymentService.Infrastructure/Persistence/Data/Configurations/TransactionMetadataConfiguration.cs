namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Configurations;

public class TransactionMetadataConfiguration : IEntityTypeConfiguration<TransactionMetadata>
{
    public void Configure(EntityTypeBuilder<TransactionMetadata> builder)
    {
        builder.ToTable("transaction_metadata");
        builder.HasKey(static t => t.Id);

        builder.Property(static t => t.Id)
            .ValueGeneratedNever()
            .HasMaxLength(100);

        builder.Property(static t => t.TransactionId)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new TransactionId(value));

        builder.Property(static t => t.Type)
            .IsRequired();

        builder.HasIndex(static t => t.TransactionId).IsUnique();

        builder.HasDiscriminator(static t => t.Type)
            .HasValue<StripeTransactionMetadata>(StripeTransactionMetadata.TypeValue);
    }
}

public class StripeTransactionMetadataConfiguration : IEntityTypeConfiguration<StripeTransactionMetadata>
{
    public void Configure(EntityTypeBuilder<StripeTransactionMetadata> builder)
    {
        builder.HasBaseType<TransactionMetadata>();

        builder.Property(static t => t.StripeCustomerId)
            .IsRequired();

    }
}
