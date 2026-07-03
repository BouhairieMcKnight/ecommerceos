using Avro.Specific;
using MassTransit;
using MassTransit.Configuration;

namespace ECommerceOS.PaymentService.Infrastructure.Messaging.Middleware;

public class AvroUnionMessageTypeFilterPipeSpecification<TAvro>(Func<TAvro, object> propertySelector) :
    IPipeSpecification<ConsumeContext>
    where TAvro : class, ISpecificRecord
{
    public void Apply(IPipeBuilder<ConsumeContext> builder)
    {
        builder.AddFilter(new AvroUnionMessageTypeFilter<TAvro>(propertySelector));
    }

    public IEnumerable<ValidationResult> Validate()
    {
        yield break;
    }
}