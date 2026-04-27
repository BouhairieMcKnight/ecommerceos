namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Configurations;

internal sealed class CategoryClosureConfiguration : IEntityTypeConfiguration<CategoryClosure>
{
    public void Configure(EntityTypeBuilder<CategoryClosure> builder)
    {
        builder.ToTable("category_closures");
        builder.Property(cc => cc.ChildCategoryId)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new CategoryId(value));
        
        builder.Property(cc => cc.ParentCategoryId)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new CategoryId(value));
        
        builder.HasKey(cc => new { cc.ParentCategoryId, cc.ChildCategoryId });
        
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.ChildCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}