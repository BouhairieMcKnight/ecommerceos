using ECommerceOS.CheckoutService;
using Grpc.Net.ClientFactory;

namespace ECommerceOS.CatalogService.Infrastructure.External;

public class CheckoutCartService(GrpcClientFactory factory) : ICheckoutCartService
{
    public async Task<Result<string>> StartCheckoutAsync(
        UserId customerId,
        List<CheckoutRequestModel> items,
        CancellationToken cancellationToken = default)
    {
        if (items.Count == 0)
        {
            return Result<string>.Failure(ProductErrors.NotValidProduct);
        }

        var client = factory.CreateClient<Checkout.CheckoutClient>("checkout");

        var request = new GetCheckoutRequest
        {
            IdempotentId = Guid.NewGuid().ToString(),
            UserId = customerId.Value.ToString(),
            Items = { items }
        };

        var response = await client.GetCheckoutSessionAsync(request, cancellationToken: cancellationToken);

        return response?.ClientSecret is null ?
            Result<string>.Failure(ProductErrors.NotFound) : 
            Result<string>.Success(response.ClientSecret);
    }
}