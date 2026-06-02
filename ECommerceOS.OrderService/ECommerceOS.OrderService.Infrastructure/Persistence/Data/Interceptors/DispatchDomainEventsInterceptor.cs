using ECommerceOS.OrderService.Infrastructure.Persistence.Data.Models;
using ECommerceOS.Shared.Contracts.Interfaces;
using ECommerceOS.Shared.Contracts.Messaging.Order;
using ECommerceOS.Shared.DTOs;

namespace ECommerceOS.OrderService.Infrastructure.Persistence.Data.Interceptors;

public class DispatchDomainEventsInterceptor(
    TimeProvider timeProvider,
    IAsyncSerializer<IIntegrationEvent> serializer) : SaveChangesInterceptor
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
        CancellationToken cancellationToken = new CancellationToken())
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
                    OrderCreatedDomainEvent @event => new OrderSubmitted
                    {
                        OrderId = @event.OrderId,
                        CustomerId = @event.CustomerId,
                        TransactionId = @event.TransactionId,
                        OrderItems = @event.OrderItems.Adapt<List<CheckoutDto>>(),
                        Version = 1,
                        CreatedAt = @event.OccurredOn.DateTime
                    },
                    OrderConfirmedDomainEvent @event => new OrderConfirmed
                    {
                        OrderId = @event.OrderId,
                        TransactionId = @event.TransactionId,
                        CreatedAt = @event.OccurredOn.DateTime,
                        ExpectedDeliveryDate = @event.ExpectedDeliveryDate,
                        CustomerId = @event.CustomerId,
                        OrderItems = @event.OrderItems.Adapt<List<CheckoutDto>>(),
                        Version = 1
                    },
                    OrderCancelDomainEvent @event => new OrderCancelled
                    {
                        OrderId = @event.OrderId,
                        TransactionId = @event.TransactionId,
                        CustomerId = @event.CustomerId,
                        OrderItems = @event.OrderItems.Adapt<List<CheckoutDto>>(),
                        Reason = @event.Reason ?? "Cancelled",
                        Version = 1,
                        CreatedAt = @event.OccurredOn.DateTime
                    },
                    _ => null
                };
                
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
