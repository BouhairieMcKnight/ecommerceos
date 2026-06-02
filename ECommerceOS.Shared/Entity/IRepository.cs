using ECommerceOS.Shared.Result;
namespace ECommerceOS.Shared.Entity;

public interface IRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId> 
    where TId : notnull
{
    Task<Result<TAggregate>> GetByIdAsync(TId id, CancellationToken ct = default);
    Task AddAsync(TAggregate entity, CancellationToken ct = default);
    Task UpdateAsync(TAggregate entity, CancellationToken ct = default);
    Task DeleteAsync(TAggregate entity, CancellationToken ct = default);
}