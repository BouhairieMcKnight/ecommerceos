using ECommerceOS.CheckoutService;
using ECommerceOS.ReservationService;
using Google.Protobuf.Collections;
using Grpc.Net.ClientFactory;
using Error = ECommerceOS.Shared.Result.Error;

namespace ECommerceOS.OrderService.Infrastructure.Grpc;

public class InventoryReservationService(GrpcClientFactory factory)
{
    public async Task<Result> ReserveInventoryAsync(
        UserId userId,
        OrderId orderId,
        List<ReservationRequestModel> reservations,
        CancellationToken cancellationToken = default
        )
    {
        var client = factory.CreateClient<Reserve.ReserveClient>("reserve");
        
        RepeatedField<ReservationRequestModel> repeatedItems = new();
        repeatedItems.AddRange(reservations);

        var request = new ReserveInventoryRequest
        {
            IdempotentId = Guid.NewGuid().ToString(),
            UserId = userId.Value.ToString(),
            Items = { repeatedItems }
        };
        
        var response = await client.ReserveInventoryAsync(request, cancellationToken: cancellationToken);

        if (response is null)
        {
            return Result.Failure(Error.Failure("Grpc Inventory Service", "Null response from service"));
        }

        return string.IsNullOrEmpty(response.Error)
            ? Result.Success()
            : Result.Failure(Error.Conflict("Grpc Inventory Service", response.Error));
    }
}
