using System.Collections.Concurrent;
using System.Net;
using Avro.IO;
using Confluent.SchemaRegistry;

namespace ECommerceOS.PaymentService.Infrastructure.Messaging.Serialization;

public class MultipleTypeDeserializer<T> : IAsyncDeserializer<T>
    {
        public const byte MagicByte = 0;
        private readonly ISchemaRegistryClient _schemaRegistryClient;
        private readonly MultipleTypeConfig _typeConfig;
        private readonly ConcurrentDictionary<int, IReaderWrapper> _readers = new();
        private readonly SemaphoreSlim _semaphore = new(1);

        public MultipleTypeDeserializer(MultipleTypeConfig typeConfig, ISchemaRegistryClient schemaRegistryClient)
        {
            _typeConfig = typeConfig;
            _schemaRegistryClient = schemaRegistryClient;
        }

        /// <summary>
        ///     Deserialize an object of one of the specified types from a byte array
        /// </summary>
        /// <param name="data">
        ///     The raw byte data to deserialize.
        /// </param>
        /// <param name="isNull">
        ///     True if this is a null value.
        /// </param>
        /// <param name="context">
        ///     Context relevant to the deserialize operation.
        /// </param>
        /// <returns>
        ///     A <see cref="System.Threading.Tasks.Task" /> that completes
        ///     with the deserialized value.
        /// </returns>
        public async Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
        {
            try
            {
                if (data.Length < 5)
                {
                    throw new InvalidDataException($"Expecting data framing of length 5 bytes or more but total data size is {data.Length} bytes");
                }

                using (var stream = new MemoryStream(data.ToArray()))
                using (var reader = new BinaryReader(stream))
                {
                    var magicByte = reader.ReadByte();
                    if (magicByte != MagicByte)
                    {
                        throw new InvalidDataException($"Expecting data with Confluent Schema Registry framing. Magic byte was {magicByte}, expecting {MagicByte}");
                    }
                    var schemaId = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    
                    var readerWrapper = await GetReader(schemaId);
                    return (T) readerWrapper.Read(new BinaryDecoder(stream));
                }
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
            
        }

        private async Task<IReaderWrapper> GetReader(int schemaId)
        {
            if (_readers.TryGetValue(schemaId, out var reader))
            {
                return reader;
            }
            // TODO - "keyed Semaphore" to download multiple schemas in parallel (currently a similar
            // approach with a single semaphore is used in Confluent.SchemaRegistry.CachedSchemaRegistryClient)
            await _semaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                if (!_readers.TryGetValue(schemaId, out reader))
                {
                    CleanCache();

                    var registrySchema = await _schemaRegistryClient.GetSchemaAsync(schemaId)
                        .ConfigureAwait(continueOnCapturedContext: false);
                    var avroSchema = Avro.Schema.Parse(registrySchema.SchemaString);
                    reader = _typeConfig.CreateReader(avroSchema);
                    _readers[schemaId] = reader;
                }
                return reader;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void CleanCache()
        {
            if (_readers.Count > _schemaRegistryClient.MaxCachedSchemas)
            {
                // LRU cache would improve performance, though there is currently
                // equally brutal logic in Confluent.SchemaRegistry.CachedSchemaRegistryClient
                _readers.Clear();
            }
        }
    }