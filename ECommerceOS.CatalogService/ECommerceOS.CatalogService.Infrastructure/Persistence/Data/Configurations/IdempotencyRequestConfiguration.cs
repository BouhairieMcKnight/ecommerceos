namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Configurations;

internal sealed class IdempotencyRequestConfiguration : IEntityTypeConfiguration<IdempotentRequest> 
{
    public void Configure(EntityTypeBuilder<IdempotentRequest> builder)
    {
        builder.ToTable("idempotency_requests");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).IsRequired();
    }
}