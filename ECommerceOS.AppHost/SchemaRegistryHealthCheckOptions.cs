using Confluent.SchemaRegistry;

namespace ECommerceOS.AppHost;

public class SchemaRegistryHealthCheckOptions
{
    public SchemaRegistryConfig Configuration { get; set; } = null!;
}