namespace ECommerceOS.OrderService.Infrastructure.Serialization;

public interface ISerializerWrapper
{
    Task<byte[]> SerializeAsync(object data, SerializationContext context);
}