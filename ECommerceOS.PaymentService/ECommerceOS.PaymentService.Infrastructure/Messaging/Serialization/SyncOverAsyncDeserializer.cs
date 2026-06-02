namespace ECommerceOS.PaymentService.Infrastructure.Messaging.Serialization;

public sealed class SyncOverAsyncDeserializer<T>(IAsyncDeserializer<T> asyncDeserializer) : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return asyncDeserializer.DeserializeAsync(data.ToArray(), isNull, context).GetAwaiter().GetResult();
    }
}
