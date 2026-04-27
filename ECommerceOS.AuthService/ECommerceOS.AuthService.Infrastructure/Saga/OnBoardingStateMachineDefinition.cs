using MassTransit;

namespace ECommerceOS.AuthService.Infrastructure.Saga;

public class OnBoardingStateMachineDefinition : SagaDefinition<OnBoardingState>
{
    private const int ConcurrencyLimit = 20;

    public OnBoardingStateMachineDefinition()
    {
        Endpoint(e =>
        {
            e.Name = "auth-onboarding-saga";
            e.PrefetchCount = ConcurrencyLimit;
        });
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<OnBoardingState> sagaConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
        base.ConfigureSaga(endpointConfigurator, sagaConfigurator, context);
    }
}
