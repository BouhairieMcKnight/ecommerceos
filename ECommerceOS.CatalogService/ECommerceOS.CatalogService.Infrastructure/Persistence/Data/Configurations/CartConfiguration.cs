using ECommerceOS.CatalogService.Domain.Carts;

namespace ECommerceOS.CatalogService.Infrastructure.Persistence.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("carts");
        builder.HasKey(cart => cart.Id);

        builder.Property(cart => cart.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new CartId(value));

        builder.Property(cart => cart.CustomerId)
            .ValueGeneratedNever()
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Ignore(cart => cart.Count);
        builder.Ignore(cart => cart.DomainEvents);

        builder.OwnsMany(cart => cart.CartItems, items =>
        {
            items.ToTable("cart_items");
            items.WithOwner().HasForeignKey(item => item.CartId);
            items.HasKey(item => item.Id);

            items.Property(item => item.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new CartItemId(value));

            items.Property(item => item.CartId)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new CartId(value));

            items.Property(item => item.ProductId)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new ProductId(value));

            items.Property(item => item.SellerId)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));

            items.Property(item => item.Price)
                .HasConversion(
                    price => price.ToString(),
                    value => Money.Create(value))
                .HasColumnName("Price");
        });
    }
}
