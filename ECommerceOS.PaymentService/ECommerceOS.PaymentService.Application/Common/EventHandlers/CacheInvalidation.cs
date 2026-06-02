namespace ECommerceOS.PaymentService.Application.Common.EventHandlers;

public class CacheInvalidation(
    ICacheService cacheService) 
    : INotificationHandler<IDomainEvent>
{
    public async Task Handle(
        IDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var tag = notification.Type;
        
        await cacheService.RemoveAsync(tag, cancellationToken);
    }
}