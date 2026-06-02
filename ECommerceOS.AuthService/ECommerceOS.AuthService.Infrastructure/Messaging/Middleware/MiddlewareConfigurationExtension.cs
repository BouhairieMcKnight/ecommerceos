using Avro.Specific;

namespace ECommerceOS.AuthService.Infrastructure.Middleware;

public static class MiddlewareConfigurationExtensions
{
    public static void UseAvroUnionMessageTypeFilter<TAvro>(
        this IConsumePipeConfigurator configurator,
        Func<TAvro, object> propertySelector)
        where TAvro : class, ISpecificRecord
    {
        configurator.AddPrePipeSpecification(new AvroUnionMessageTypeFilterPipeSpecification<TAvro>(propertySelector));
    }
}
