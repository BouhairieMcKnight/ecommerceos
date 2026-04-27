namespace ECommerceOS.PaymentService.Infrastructure.Interfaces;

public interface IAccountCreatedConsumer
{
    public Task ConsumeMessageAsync(CancellationToken cancellationToken);
}