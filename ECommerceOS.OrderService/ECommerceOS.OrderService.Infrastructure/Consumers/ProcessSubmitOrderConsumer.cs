using ECommerceOS.OrderService.Application.Orders.Command.SubmitOrder;
using ECommerceOS.OrderService.Infrastructure.StateMachines;
using MassTransit;
using MediatR;

namespace ECommerceOS.OrderService.Infrastructure.Consumers;

public class ProcessSubmitOrderConsumer(IMediator mediator) : IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        var command = new SubmitOrderCommand(
            context.Message.OrderId,
            context.Message.CustomerId,
            context.Message.TransactionId,
            context.Message.ShippingAddress,
            context.Message.OrderItems);

        var result = await mediator.Send(command, context.CancellationToken);
        if (result.IsSuccess)
        {
            await context.Publish<SubmitOrderSucceeded>(new
            {
                context.Message.OrderId
            }, context.CancellationToken);
            return;
        }

        await context.Publish<SubmitOrderFailed>(new
        {
            context.Message.OrderId,
            Reason = result.Error?.Description
        }, context.CancellationToken);
    }
}
