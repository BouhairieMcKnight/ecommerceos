namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payment_Methods");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new PaymentId(value));

        builder.Property(p => p.UserId)
            .ValueGeneratedNever()
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(p => p.Name)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(p => p.PaymentStatus)
            .IsRequired()
            .HasConversion(
                status => status.ToString(),
                status => Enum.Parse<PaymentStatus>(status));
        
        builder.HasOne(p => p.PaymentMetadata)
            .WithOne()
            .HasForeignKey<PaymentMetadata>(m => m.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(p => p.DomainEvents);
    }
}
