using System.Text.Json.Serialization;

namespace ECommerceOS.Shared.ValueObjects;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<OrderId, Guid>))]
public record OrderId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => Value.ToString();
}