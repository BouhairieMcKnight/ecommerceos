namespace ECommerceOS.OrderService.Infrastructure.Persistence.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(order => order.Id);
        
        builder.Property(order => order.CustomerId)
            .ValueGeneratedNever()
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));
        
        builder.Property(order => order.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new OrderId(value));
        
        builder.Property(order => order.TransactionId)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new TransactionId(value));

        builder.OwnsOne(order => order.Address, address =>
            {
                address.Property(add => add.Street).HasMaxLength(50);
                address.Property(add => add.City).HasMaxLength(50);
                address.Property(add => add.Country).HasMaxLength(50);
                address.Property(add => add.State).HasMaxLength(15);
                address.Property(add => add.ZipCode).HasMaxLength(10);
            });

        builder.OwnsMany(order => order.OrderItems, items=>
            {
                items.ToTable("OrderItems");
                items.WithOwner().HasForeignKey(item => item.OrderId);
                items.HasKey(item => item.Id);
                items.Property(orderItem => orderItem.Id)
                    .ValueGeneratedNever()
                    .HasConversion(
                        id => id.Value,
                        value => new OrderItemId(value));
                
                items.Property(orderItem => orderItem.SellerId)
                    .ValueGeneratedNever()
                    .IsRequired()
                    .HasConversion(
                        id => id.Value,
                        value => new UserId(value));
                
                items.Property(orderItem => orderItem.Price)
                    .HasConversion(
                        price => price.ToString(),
                        value => Money.Create(value));
                
                items.Property(orderItem => orderItem.ProductId)
                    .ValueGeneratedNever()
                    .IsRequired()
                    .HasConversion(
                        id => id.Value,
                        id => new ProductId(id));
            });
        
        builder.Navigation(order => order.OrderItems)
            .HasField("_orderItems")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Ignore(order => order.Completed);
        builder.Ignore(order => order.DomainEvents);
        builder.Ignore(order => order.TotalPrice);
    }
}
