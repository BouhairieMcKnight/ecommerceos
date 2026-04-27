using ECommerceOS.OrderService.Application.Orders.Command.ConfirmOrder;
using ECommerceOS.OrderService.Infrastructure.StateMachines;
using MassTransit;
using MediatR;

namespace ECommerceOS.OrderService.Infrastructure.Consumers;

public class ProcessConfirmOrderConsumer(IMediator mediator) : IConsumer<ConfirmOrder>
{
    public async Task Consume(ConsumeContext<ConfirmOrder> context)
    {
        var command = new ConfirmOrderCommand(
            context.Message.OrderId,
            context.Message.ExpectedDeliveryDate);

        var result = await mediator.Send(command, context.CancellationToken);
        if (result.IsSuccess)
        {
            await context.Publish<ConfirmOrderSucceeded>(new
            {
                context.Message.OrderId,
                context.Message.ExpectedDeliveryDate
            }, context.CancellationToken);
            return;
        }
        
        await context.Publish<ConfirmOrderFailed>(new
        {
            context.Message.OrderId,
            Reason = result.Error?.Description
        }, context.CancellationToken);
    }
}
