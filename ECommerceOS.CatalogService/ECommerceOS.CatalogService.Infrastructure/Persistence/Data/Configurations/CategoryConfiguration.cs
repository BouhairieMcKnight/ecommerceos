namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new CategoryId(value));

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(c => c.Title)
            .IsUnique();
        
        builder.Property(c => c.ParentCategoryId)
            .ValueGeneratedNever()
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value.HasValue ? new CategoryId(value.Value) : null);

        builder.HasMany(c => c.ChildrenCategories)
            .WithOne(c => c.ParentCategory)
            .HasForeignKey(c => c.ParentCategoryId);
        
        builder.Navigation(c => c.ChildrenCategories)
            .HasField("_childrenCategories")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Ignore(c => c.DomainEvents);
    }
}
