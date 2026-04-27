using System.Text.Json.Serialization;

namespace ECommerceOS.Shared.ValueObjects;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<PaymentId, Guid>))]
public record PaymentId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => Value.ToString();
}