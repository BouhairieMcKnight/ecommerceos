using ECommerceOS.Shared.DTOs;

namespace ECommerceOS.CatalogService.Domain.Carts;

public record CartClearedDomainEvent : IDomainEvent
{
    public string Type => nameof(Cart);
    public required CartId CartId { get; init; }
    public required List<CheckoutDto> CheckoutItems { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}

public record CartCheckoutDomainEvent : IDomainEvent
{
    public string Type => nameof(Cart);
    public required UserId CustomerId { get; init; }
    public required CartId CartId { get; init; }
    public required List<CheckoutDto> CheckoutItems { get; init; }
    public required DateTimeOffset OccurredOn { get; init; }
}

public record CartProductsReservedDomainEvent : IDomainEvent
{
    public string Type => nameof(Cart);
    public required CartId CartId { get; init; }
    public required OrderId OrderId { get; init; }
    public required List<CheckoutDto> CheckoutItems { get; init; }
    public DateTimeOffset OccurredOn { get; init; }
}
