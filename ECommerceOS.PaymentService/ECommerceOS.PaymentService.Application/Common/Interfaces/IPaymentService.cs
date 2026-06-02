namespace ECommerceOS.PaymentService.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<Result> DeletePaymentAsync(PaymentId paymentId, CancellationToken cancellationToken = default);

    Task<string> RegisterCustomerAsync(UserId userId, string email, string name,
        CancellationToken cancellationToken =  default);

    Task<Result<string>> RegisterPaymentMethodAsync(
        UserId userId, string paymentMethod, CancellationToken cancellationToken = default);
}