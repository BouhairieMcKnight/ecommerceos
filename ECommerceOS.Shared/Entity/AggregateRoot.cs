using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceOS.Shared.Entity;

public abstract class AggregateRoot<TId> 
    : BaseEntity<TId>, IAggregateRoot
    where TId : notnull
{
    // ReSharper disable once InconsistentNaming
    protected readonly List<IDomainEvent> domainEvents = [];

    [NotMapped]
    public IEnumerable<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Remove(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}

public interface IAggregateRoot
{
    IEnumerable<IDomainEvent> DomainEvents { get; }
    void AddDomainEvent(IDomainEvent domainEvent);
    void RemoveDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}
