namespace ECommerceOS.PaymentService.Infrastructure.Messaging.Serialization;

public interface ISerializerWrapper
{
    Task<byte[]> SerializeAsync(object data, SerializationContext context);
}