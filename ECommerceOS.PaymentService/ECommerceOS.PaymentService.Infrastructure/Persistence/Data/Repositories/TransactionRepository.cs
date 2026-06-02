using ECommerceOS.PaymentService.Application.Common.Interfaces;
using ECommerceOS.Shared.Result;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerceOS.PaymentService.Infrastructure.Persistence.Data.Repositories;

public class TransactionRepository(PaymentDbContext dbContext) : ITransactionRepository
{
    public async Task<Result<Transaction>> GetByIdAsync(TransactionId id, CancellationToken ct = new CancellationToken())
    {
        var transaction = await dbContext.Set<Transaction>()
            .FirstOrDefaultAsync(t => t.Id == id, ct);
        
        return transaction is null ? 
            Result<Transaction>.Failure(TransactionErrors.NotFound) :
            Result<Transaction>.Success(transaction);
    }

    public async Task AddAsync(Transaction entity, CancellationToken ct = new CancellationToken())
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            await dbContext.Set<Transaction>().AddAsync(entity, token); 
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<Transaction>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task UpdateAsync(Transaction entity, CancellationToken ct = new CancellationToken())
    {
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Transaction entity, CancellationToken ct = new CancellationToken())
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            dbContext.Set<Transaction>().Remove(entity);
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<Transaction>().AnyAsync(t => t.Id == entity.Id, token);
        }, ct);
    }

    public async Task<bool> VerifyTransactionAsync(TransactionId transactionId, UserId userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<Transaction>()
            .AnyAsync(t => t.Id == transactionId && t.CustomerId == userId, cancellationToken);
    }
}
