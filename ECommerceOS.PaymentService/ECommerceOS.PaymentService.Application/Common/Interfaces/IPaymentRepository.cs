using ECommerceOS.PaymentService.Domain.Payments;

namespace ECommerceOS.PaymentService.Application.Common.Interfaces;

public interface IPaymentRepository : IRepository<Payment, PaymentId>
{
    Task<bool> VerifyPaymentAsync(PaymentId paymentId, UserId userId, CancellationToken cancellationToken = default);
    Task<bool> VerifyUserPaymentAsync(UserId userId, CancellationToken cancellationToken = default);
    IQueryable<Payment> QueryByUser(UserId userId);
}
