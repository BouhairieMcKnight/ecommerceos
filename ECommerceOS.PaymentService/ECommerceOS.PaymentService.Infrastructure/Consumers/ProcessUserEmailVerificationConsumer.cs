using ECommerceOS.Shared.Contracts.Messaging.Identity;
using MassTransit;

namespace ECommerceOS.PaymentService.Infrastructure.Consumers;

public class ProcessUserEmailVerificationConsumer(
    IPaymentService paymentService,
    ILogger<ProcessUserEmailVerificationConsumer> logger)
    : IConsumer<UserEmailVerified>
{
    public async Task Consume(ConsumeContext<UserEmailVerified> context)
    {
        logger.LogInformation("ProcessUserEmailVerificationConsumer received UserEmailVerified");
        
        await paymentService.RegisterCustomerAsync(
            context.Message.UserId!,
            context.Message.Email,
            context.Message.Name,
            context.CancellationToken);

        await context.ConsumeCompleted;
    }
}
