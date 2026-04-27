namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(trx => trx.Id);
        builder.Property(trx => trx.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new TransactionId(value));

        builder.Property(trx => trx.CustomerId)
            .ValueGeneratedNever()
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));
        
        builder.Property(trx => trx.CustomerPaymentId)
            .ValueGeneratedNever()
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new PaymentId(value));
        
        builder.Property(trx => trx.OrderId)
            .ValueGeneratedNever()
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new OrderId(value));
        
        builder.Property(trx => trx.Status)
            .HasConversion(
                status => status.ToString(),
                str => Enum.Parse<TransactionStatus>(str));

        builder.OwnsMany(trx => trx.TransactionItems, items=>
        {
            items.ToTable("TransactionItems");
            items.WithOwner().HasForeignKey(item => item.TransactionId);
            items.HasKey(item => item.Id);
            items.Property(item => item.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    id => new TransactionItemId(id));

            items.Property(item => item.SellerId)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));

            items.Property(item => item.ProductId)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new ProductId(value));

            items.Property(item => item.Amount)
                .HasConversion(
                    amount => amount.ToString(),
                    value => Money.Create(value));
        });

        builder.HasAlternateKey(trx => new { trx.CustomerId, trx.OrderId });
        builder.HasIndex(trx => trx.OrderId).IsUnique();

        // Transaction metadata is stored independently before the aggregate is created
        // (checkout session starts before webhook-driven transaction creation).
        builder.Ignore(trx => trx.TransactionMetadata);

        builder.Ignore(trx => trx.DomainEvents);
    }
}
