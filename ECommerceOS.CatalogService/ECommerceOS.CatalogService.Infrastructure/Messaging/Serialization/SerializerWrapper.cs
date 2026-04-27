using Confluent.SchemaRegistry.Serdes;

namespace ECommerceOS.CatalogService.Infrastructure.Messaging.Serialization;

internal class SerializerWrapper<T> : ISerializerWrapper
{
    private readonly AvroSerializer<T> _inner;

    public SerializerWrapper(AvroSerializer<T> inner)
    {
        _inner = inner;
    }

    public async Task<byte[]> SerializeAsync(object data, SerializationContext context)
    {
        return await _inner.SerializeAsync((T)data, context).ConfigureAwait(false);
    }
}