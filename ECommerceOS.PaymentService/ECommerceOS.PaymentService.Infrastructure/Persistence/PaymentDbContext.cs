using ECommerceOS.PaymentService.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace ECommerceOS.PaymentService.Infrastructure.Persistence;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)  
{
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<OutboxMessage> OutBoxMessages { get; set; }
    public DbSet<StripeAccount> StripeAccounts { get; set; }
    public DbSet<PaymentMetadata> PaymentMetadata { get; set; }
    public DbSet<TransactionMetadata> TransactionMetadata { get; set; }
    public DbSet<IdempotentRequest> IdempotentRequests { get; set; }

    static PaymentDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("outbox_messages");
            builder.HasKey(x => x.MessageId);
            builder.Property(x => x.MessageId).HasColumnName("message_id");
            builder.Property(x => x.Type).HasColumnName("type");
            builder.Property(x => x.IntegrationEvent).HasColumnName("integration_event");
            builder.Property(x => x.Attempts).HasColumnName("attempts");
            builder.Property(x => x.Error).HasColumnName("error");
            builder.Property(x => x.ProcessedOn).HasColumnName("processed_on");
            builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<StripeAccount>(builder =>
        {
            builder.ToTable("stripe_accounts");
            builder.HasKey(x => x.UserId);
            builder.Property(x => x.UserId)
                .ValueGeneratedNever()
                .HasConversion(id => id.Value, value => new UserId(value));
            builder.Property(x => x.AccountId)
                .IsRequired();
        });

        modelBuilder.Entity<IdempotentRequest>(builder =>
        {
            builder.ToTable("idempotent_requests");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            builder.Property(x => x.Name)
                .HasColumnName("name")
                .IsRequired();
            builder.Property(x => x.CreatedOnUtc)
                .HasColumnName("created_on_utc");
        });
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
    }
}
