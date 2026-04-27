using Avro.IO;

namespace ECommerceOS.PaymentService.Infrastructure.Messaging.Serialization;

public interface IReaderWrapper
{
    object Read(BinaryDecoder decoder);
}