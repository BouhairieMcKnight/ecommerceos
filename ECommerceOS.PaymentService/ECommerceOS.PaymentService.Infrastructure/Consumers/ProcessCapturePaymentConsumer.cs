using ECommerceOS.PaymentService.Infrastructure.StateMachines;
using ECommerceOS.Shared.Contracts.Messaging.Payment;
using MassTransit;

namespace ECommerceOS.PaymentService.Infrastructure.Consumers;

public class ProcessCapturePaymentConsumer(
    ITransactionService transactionService) : IConsumer<CapturePayment>
{
    public async Task Consume(ConsumeContext<CapturePayment> context)
    {
        try
        {
            await transactionService.CapturePaymentAsync(
                context.Message.TransactionId,
                context.Message.Amount,
                context.Message.IdempotencyKey,
                context.CancellationToken);
        }
        catch (Exception ex)
        {
            await context.Publish(new TransactionFailed
            {
                TransactionId = context.Message.TransactionId,
                CustomerId = context.Message.CustomerId,
                Reason = ex.Message,
                Version = 1,
                CreatedAt = DateTimeOffset.UtcNow.DateTime
            }, context.CancellationToken);
        }
    }
}
