namespace ECommerceOS.OrderService.Application.Common.Interfaces;

public interface IIdempotencyService
{
    Task<bool> RequestExistsAsync(Guid requestId);
    Task CreateRequestAsync(Guid requestId, string requestName);
}