using System.Text.Json.Serialization;

namespace ECommerceOS.Shared.ValueObjects;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<OrderItemId, Guid>))]
public record OrderItemId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => Value.ToString();
}