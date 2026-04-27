using Avro.IO;

namespace ECommerceOS.CatalogService.Infrastructure.Messaging.Serialization;

public interface IReaderWrapper
{
    object Read(BinaryDecoder decoder);
}