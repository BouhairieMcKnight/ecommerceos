using ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Models;

namespace ECommerceOS.PaymentService.Infrastructure.Idempotency;

public class IdempotencyService(
    PaymentDbContext dbContext) : IIdempotencyService
{
    public async Task<bool> RequestExistsAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<IdempotentRequest>().AsNoTracking()
            .AnyAsync(i => i.Id == requestId, cancellationToken);
    }

    public async Task CreateRequestAsync(Guid requestId, string requestName,
        CancellationToken cancellationToken = default)
    {
        var idempotentRequest = new IdempotentRequest
        {
            Id = requestId,
            Name = requestName,
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        
        await dbContext.Set<IdempotentRequest>().AddAsync(idempotentRequest, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
