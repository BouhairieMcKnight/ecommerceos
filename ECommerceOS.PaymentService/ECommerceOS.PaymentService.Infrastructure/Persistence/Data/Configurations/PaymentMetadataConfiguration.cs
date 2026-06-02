namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Configurations;

public class PaymentMetadataConfiguration : IEntityTypeConfiguration<PaymentMetadata>
{
    public void Configure(EntityTypeBuilder<PaymentMetadata> builder)
    {
        builder.ToTable("payment_metadata");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasMaxLength(100);

        builder.Property(x => x.PaymentId)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new PaymentId(value));
        
        
        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.PaymentMethod)
            .IsRequired();

        builder.HasIndex(x => x.PaymentId).IsUnique();

        builder.HasDiscriminator(x => x.Type)
            .HasValue<StripePaymentMetadata>(StripePaymentMetadata.TypeValue);
    }
}

public class StripePaymentMetadataConfiguration : IEntityTypeConfiguration<StripePaymentMetadata>
{
    public void Configure(EntityTypeBuilder<StripePaymentMetadata> builder)
    {
        builder.HasBaseType<PaymentMetadata>();

        builder.Property(x => x.StripeCustomerId)
            .IsRequired();
        
        builder.Property(x => x.PaymentId)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new PaymentId(value));
    }
}
