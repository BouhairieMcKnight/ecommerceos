using MediatR;

namespace ECommerceOS.Shared.Entity;

public interface IDomainEvent : INotification
{
    public string Type { get; }
    public DateTimeOffset OccurredOn { get; init; }
}