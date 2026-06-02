namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("catalog_products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .HasMaxLength(100);
        builder.Property(p => p.Sku)
            .HasConversion(
                sku => sku.Value,
                value => Sku.Create(value)!)
            .HasMaxLength(15);

        builder.Property(p => p.Price)
            .HasConversion(
                price => price.ToString(),
                value => Money.Create(value));
        
        builder.Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ProductId(value));

        builder.HasMany(p => p.ProductCategories)
            .WithOne()
            .HasForeignKey(pc => pc.ProductId);
        
        builder.Ignore(p => p.ImageUrls);
        
        builder.Navigation(p => p.ProductCategories)
            .HasField("_productCategories")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Property(p => p.SellerId)
            .ValueGeneratedNever()
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Ignore(p => p.DomainEvents);
    }
}
