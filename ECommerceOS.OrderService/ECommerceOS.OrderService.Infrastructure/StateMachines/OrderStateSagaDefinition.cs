using MassTransit;

namespace ECommerceOS.OrderService.Infrastructure.StateMachines;

public sealed class OrderStateSagaDefinition : SagaDefinition<OrderState>
{
    private const int ConcurrencyLimit = 20;

    public OrderStateSagaDefinition()
    {
        Endpoint(e =>
        {
            e.Name = "saga-queue";
            e.PrefetchCount = ConcurrencyLimit;
        });
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<OrderState> sagaConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        
        var partition = endpointConfigurator.CreatePartitioner(ConcurrencyLimit);
        
        base.ConfigureSaga(endpointConfigurator, sagaConfigurator, context);
    }
}