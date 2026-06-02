using System.Text.Json.Serialization;

namespace ECommerceOS.Shared.ValueObjects;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<CartItemId, Guid>))]
public record CartItemId(Guid Value) : StronglyTypedId<Guid>(Value) ;