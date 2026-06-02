using ECommerceOS.OrderService.Infrastructure.Persistence.Data.Models;

namespace ECommerceOS.OrderService.Infrastructure.Idempotency;

public class IdempotencyService(
    OrderDbContext dbContext) : IIdempotencyService
{
    public async Task<bool> RequestExistsAsync(Guid requestId)
    {
        return await dbContext.Set<IdempotentRequest>().AsNoTracking()
            .AnyAsync(i => i.Id == requestId);
    }

    public async Task CreateRequestAsync(Guid requestId, string requestName)
    {
        var idempotentRequest = new IdempotentRequest
        {
            Id = requestId,
            Name = requestName,
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
        
        await dbContext.Set<IdempotentRequest>().AddAsync(idempotentRequest);
        await dbContext.SaveChangesAsync();
    }
}