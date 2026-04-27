using System.Text.Json.Serialization;

namespace ECommerceOS.Shared.ValueObjects;

[JsonConverter(typeof(StronglyTypedIdJsonConverter<TransactionId, Guid>))]
public record TransactionId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => Value.ToString();
}