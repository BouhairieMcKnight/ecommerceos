namespace ECommerceOS.Shared.Contracts.Interfaces;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent;
}