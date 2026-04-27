using System.Text.Json.Serialization;

namespace ECommerceOS.Shared.ValueObjects;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<CartId, Guid>))]
public record CartId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => Value.ToString();
}