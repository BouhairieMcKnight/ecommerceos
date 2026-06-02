namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Configurations;

internal sealed class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("product_categories");
        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ProductCategoryId(value));

        builder.Property(pc => pc.ProductId)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ProductId(value));

        builder.Property(pc => pc.CategoryId)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new CategoryId(value));

        builder.HasOne<Product>()
            .WithMany(product => product.ProductCategories)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(pc => pc.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
