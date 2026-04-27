using Avro.IO;

namespace ECommerceOS.OrderService.Infrastructure.Serialization;

public interface IReaderWrapper
{
    object Read(BinaryDecoder decoder);
}