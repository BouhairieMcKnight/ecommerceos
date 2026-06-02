using ECommerceOS.OrderService.Infrastructure.Grpc;
using ECommerceOS.OrderService.Infrastructure.StateMachines;
using ECommerceOS.ReservationService;
using MassTransit;

namespace ECommerceOS.OrderService.Infrastructure.Consumers;

public class ProcessInventoryReservationConsumer(
    InventoryReservationService reservationService
    )
    : IConsumer<ReserveInventory>
{
    public async Task Consume(ConsumeContext<ReserveInventory> context)
    {
        var reservations = context.Message.ReserveProducts
            .Select(o => new ReservationRequestModel
            {
                ProductId = o.ProductId.Value.ToString(),
                Quantity = o.Quantity
            })
            .ToList();

        var response = await reservationService.ReserveInventoryAsync(
            context.Message.CustomerId,
            context.Message.OrderId,
            reservations,
            context.CancellationToken);

        if (response.IsSuccess)
        {
            await context.Publish<ReserveSucceeded>(new
            {
                context.Message.OrderId
            }, context.CancellationToken);
            return;
        }

        await context.Publish<ReserveFailed>(new
                {
                    context.Message.OrderId,
                    Reason = response.Error?.Description
                },
            context.CancellationToken);
    }
}
