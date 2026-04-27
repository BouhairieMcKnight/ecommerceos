namespace ECommerceOS.CatalogService.Domain.Products;

public record ProductAddedDomainEvent : IDomainEvent
{
    public string Type =>  nameof(Product); 
    public required ProductId ProductId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required Money Price { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}

public record ProductDeletedDomainEvent(Product Product) : IDomainEvent
{
    public string Type => nameof(Product);
    public DateTimeOffset OccurredOn { get; init; }
}

public record ProductReservedDomainEvent : IDomainEvent
{
    public string Type => nameof(Product);
    public required ProductId ProductId { get; init; }
    public required CartId CartId { get; init; }
    public int QuantityReserve { get; init; }
    public required Money Cost { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
    public required UserId SellerId { get; init; }
}

public record ProductFailedReserveDomainEvent : IDomainEvent
{
    public string Type => nameof(Product);
    public required ProductId ProductId { get; init; }
    public int RemainingQuantity { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}
