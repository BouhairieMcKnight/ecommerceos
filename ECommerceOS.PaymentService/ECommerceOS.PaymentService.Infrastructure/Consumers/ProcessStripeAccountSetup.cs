using MassTransit;

namespace ECommerceOS.PaymentService.Infrastructure.Consumers;

public class ProcessStripeAccountSetup(PaymentDbContext dbContext) : IConsumer<SetupStripeAccount>
{
    public async Task Consume(ConsumeContext<SetupStripeAccount> context)
    {
        var stripeAccount = StripeAccount.Create(context.Message.UserId, context.Message.AccountId);
        
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteInTransactionAsync(async (token) =>
        {
            await dbContext.Set<StripeAccount>().AddAsync(stripeAccount, token); 
            await dbContext.SaveChangesAsync(token);
        }, async (token) =>
        {
            return await dbContext.Set<StripeAccount>().AnyAsync(s => s.UserId == stripeAccount.UserId, token);
        }, context.CancellationToken);

        await context.ConsumeCompleted;
    }
}

public record SetupStripeAccount
{
    public UserId UserId { get; init; }
    public string AccountId { get; init; }
    public DateTime CreatedAt { get; init; }
}