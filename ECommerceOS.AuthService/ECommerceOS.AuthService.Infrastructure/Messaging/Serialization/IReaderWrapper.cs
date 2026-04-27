using Avro.IO;

namespace ECommerceOS.AuthService.Infrastructure.Messaging.Serialization;

public interface IReaderWrapper
{
    object Read(BinaryDecoder decoder);
}
