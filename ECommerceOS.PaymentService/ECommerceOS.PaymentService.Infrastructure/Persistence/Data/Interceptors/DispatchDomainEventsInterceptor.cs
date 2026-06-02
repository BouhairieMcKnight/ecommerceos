using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Order;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using ECommerceOS.Shared.DTOs;
using Mapster;
using MassTransit;

namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Interceptors;

public class DispatchDomainEventsInterceptor(
    TimeProvider timeProvider,
    IAsyncSerializer<IIntegrationEvent> serializer,
    IPublishEndpoint publisher) : SaveChangesInterceptor
{
    private static readonly SerializationContext ValueContext = new(MessageComponentType.Value, "identity");

    // public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
    //     InterceptionResult<int> result)
    // {
    //     var dbContext = eventData.Context;
    //     ConvertToOutbox(dbContext);
    //
    //     return base.SavingChanges(eventData, result);
    // }
    
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        var dbContext = eventData.Context;
        await ConvertToOutbox(dbContext);
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task ConvertToOutbox(DbContext?  dbContext) 
    {
        if (dbContext == null)
        {
            return;
        }

        var outBoxMessages = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .SelectMany(aggregate =>
            {
                List<IDomainEvent> domainEvents = aggregate.DomainEvents.ToList();
                aggregate.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent =>
            {
                IIntegrationEvent? integrationEvent = domainEvent switch
                {
                    TransactionCreatedDomainEvent @event => new TransactionSubmitted
                    {
                        OrderId = @event.OrderId,
                        CustomerId = @event.CustomerId,
                        TransactionItems = @event.TransactionItems.Adapt<List<CheckoutDto>>(),
                        TransactionId = @event.TransactionId,
                        CreatedAt = @event.OccurredOn.DateTime,
                        Address = @event.Address
                    },
                    TransactionConfirmedDomainEvent @event => new TransactionConfirmed
                    {
                        OrderId = @event.OrderId,
                        TransactionId = @event.TransactionId,
                        CreatedAt = @event.OccurredOn.DateTime, 
                        Total = @event.Total,
                        CustomerId = @event.CustomerId,
                    },
                    _ => null
                };

                publisher.Publish(domainEvent);
                
                return integrationEvent;
            }).Where(integrationEvent => integrationEvent is not null)
            .Select(async integrationEvent => new OutboxMessage
            {
                MessageId = Guid.NewGuid(),
                Type = integrationEvent!.GetType().Name,
                CreatedAt = timeProvider.GetUtcNow().DateTime,
                IntegrationEvent = await serializer.SerializeAsync(integrationEvent, ValueContext)
            }).ToList();
        
        var messages = await Task.WhenAll(outBoxMessages);
        
        await dbContext.Set<OutboxMessage>().AddRangeAsync(messages);
    }
}