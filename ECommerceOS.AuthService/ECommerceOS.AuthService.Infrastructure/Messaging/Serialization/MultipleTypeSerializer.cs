using Avro.Specific;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace ECommerceOS.AuthService.Infrastructure.Messaging.Serialization;

public class MultipleTypeSerializer<T> : IAsyncSerializer<T>
{
    private readonly Dictionary<string, ISerializerWrapper> _serializers = [];
    private readonly MultipleTypeConfig _typeConfig;
    private readonly ISchemaRegistryClient _schemaRegistryClient;
    private readonly AvroSerializerConfig _serializerConfig;

    public MultipleTypeSerializer(
        MultipleTypeConfig typeConfig,
        ISchemaRegistryClient schemaRegistryClient,
        AvroSerializerConfig serializerConfig = null!)
    {
        _typeConfig = typeConfig;
        _schemaRegistryClient = schemaRegistryClient;
        _serializerConfig = serializerConfig;
        InitialiseSerializers();
    }

    private void InitialiseSerializers()
    {
        foreach (var typeInfo in _typeConfig.Types)
        {
            var serializer = typeInfo.CreateSerializer(_schemaRegistryClient, _serializerConfig);
            _serializers[typeInfo.Schema.Fullname] = serializer;
        }
    }

    public async Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        if (data is not ISpecificRecord record)
        {
            throw new ArgumentException(
                $"Object being serialized is not an instance of {nameof(ISpecificRecord)}. This serializer only serializes types generated using the avrogen.exe tool.",
                nameof(data));
        }

        var fullName = record.Schema.Fullname;
        if (!_serializers.TryGetValue(fullName, out var serializer))
        {
            throw new ArgumentException(
                $"Unexpected type {fullName}. All types to be serialized need to be registered in the {nameof(MultipleTypeConfig)} that is supplied to this instance of {nameof(MultipleTypeSerializer<T>)}",
                nameof(data));
        }

        return await serializer.SerializeAsync(data, context);
    }
}
