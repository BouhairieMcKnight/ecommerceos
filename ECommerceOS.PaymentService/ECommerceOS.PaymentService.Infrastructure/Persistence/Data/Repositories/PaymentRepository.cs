namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Repositories;

public class PaymentRepository(PaymentDbContext dbContext) : IPaymentRepository
{
    public async Task<Result<Payment>> GetByIdAsync(PaymentId id, CancellationToken ct = new CancellationToken())
    {
        var payment = await dbContext.Payments
            .Include(p => p.PaymentMetadata)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        
        return payment is null ? Result<Payment>.Failure(PaymentErrors.NotFound)  : Result<Payment>.Success(payment);
    }

    public async Task AddAsync(Payment entity, CancellationToken ct = new CancellationToken())
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            await dbContext.Set<Payment>().AddAsync(entity, token); 
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<Payment>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task UpdateAsync(Payment entity, CancellationToken ct = new CancellationToken())
    {
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Payment entity, CancellationToken ct = new CancellationToken())
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            dbContext.Set<Payment>().Remove(entity); 
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<Payment>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task<bool> VerifyPaymentAsync(PaymentId paymentId, UserId userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Payment>()
            .AnyAsync(p => p.Id == paymentId && p.UserId == userId, cancellationToken);
    }

    public async Task<bool> VerifyUserPaymentAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Payment>().AnyAsync(p => p.UserId == userId, cancellationToken);
    }

    public IQueryable<Payment> QueryByUser(UserId userId)
    {
        return dbContext.Set<Payment>()
            .AsNoTracking()
            .Include(p => p.PaymentMetadata)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedOn);
    }
}
