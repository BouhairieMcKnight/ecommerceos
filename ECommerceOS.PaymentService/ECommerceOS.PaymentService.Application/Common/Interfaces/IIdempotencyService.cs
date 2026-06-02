namespace ECommerceOS.PaymentService.Application.Common.Interfaces;

public interface IIdempotencyService
{
    Task<bool> RequestExistsAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task CreateRequestAsync(Guid requestId, string requestName, CancellationToken cancellationToken = default);
}